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
        private static Dictionary<int, int> maxCompositionDict = 
            new Dictionary<int, int>(); // key: uid

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

        public static void BuildMaxCompositionDict(LevelInfoSO levelInfo)
        {
            maxCompositionDict.Clear();
            foreach (var optionalRecipe in levelInfo.optionalRecipeMatchListItems)
            {
                ICustomRecipeOptional recipe = optionalRecipe as ICustomRecipeOptional;
                if (recipe == null) continue;
                int uid = (recipe as CustomRecipeSO).uID;
                if (maxCompositionDict.ContainsKey(uid)) continue;

                Func<ICustomRecipeOptional, CustomRecipeOptionalImpl> creator;
                if (impls.TryGetValue(recipe.GetType(), out creator))
                {
                    int capacity = creator(recipe).GetMaxComposition();
                    if (capacity > 0)
                        maxCompositionDict.Add(uid, capacity);
                }
                else
                {
                    Debug.LogWarning(recipe.GetType().Name + " not registered");
                }
            }
        }

        public static int GetMaxComposition(OrderDefinitionNode node)
        {
            if (maxCompositionDict.ContainsKey(node.m_uID))
                return maxCompositionDict[node.m_uID];
            else return 0;
        }
    }
}