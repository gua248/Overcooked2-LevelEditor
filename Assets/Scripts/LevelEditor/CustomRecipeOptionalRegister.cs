using LevelEditorStub;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public static class CustomRecipeOptionalRegister
    {
        private static readonly Dictionary<Type, Func<ICustomRecipeOptional, CustomRecipeOptionalImpl>> impls = 
            new Dictionary<Type, Func<ICustomRecipeOptional, CustomRecipeOptionalImpl>>();

        public static void Register<T, TImpl>()
            where T : CustomRecipeSO, ICustomRecipeOptional
            where TImpl : CustomRecipeOptionalImpl, new()
        {
            if (impls.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException("重复的 optional 菜单类型注册：" + typeof(T).Name);
            }

            impls.Add(typeof(T), recipe =>
            {
                TImpl impl = new TImpl();
                impl.Init(recipe);
                return impl;
            });
        }

        public static OrderDefinitionNode GetOrderDefinitionNode(this ICustomRecipeOptional recipe)
        {
            Func<ICustomRecipeOptional, CustomRecipeOptionalImpl> creator;
            if (impls.TryGetValue(recipe.GetType(), out creator))
            {
                return creator(recipe).GetOrderDefinitionNode();
            }
            else
            {
                Debug.LogWarning(recipe.GetType().Name + " not registered");
                return null;
            }
        }

        public static GameObject GetIngredientPrefab(this ICustomRecipeOptional recipe, GameObject originalPrefab)
        {
            Func<ICustomRecipeOptional, CustomRecipeOptionalImpl> creator;
            if (impls.TryGetValue(recipe.GetType(), out creator))
            {
                return creator(recipe).GetIngredientPrefab(originalPrefab);
            }
            else
            {
                Debug.LogWarning(recipe.GetType().Name + " not registered");
                return null;
            }
        }
    }
}