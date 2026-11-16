using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditorStub
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class PseudoPrefabSOCategoryAttribute : PropertyAttribute
    {
        public string Name { get; private set; }

        public PseudoPrefabSOCategoryAttribute(string name)
        {
            Name = name;
        }
    }
}