using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

namespace LevelEditor
{
    public static class ModuleManager
    {
        private class ModuleEntry
        {
            public string id;
            public Type componentType;
            public SynchroniserConfig config;
        }

        private static readonly List<ModuleEntry> entries = new List<ModuleEntry>();

        public static void Register(string id, Type componentType, SynchroniserConfig config)
        {
            if (entries.Exists(e => e.id == id))
                throw new InvalidOperationException("重复的同步组件注册 ID：" + id);

            if (entries.Exists(e => e.componentType == componentType))
                throw new InvalidOperationException("同步组件类型已注册：" + componentType.FullName);

            entries.Add(new ModuleEntry
            {
                id = id,
                componentType = componentType,
                config = config
            });
        }

        public static void Apply(EntitySerialisationRegistry entitySerialisationRegistry)
        {
            foreach (ModuleEntry entry in entries.OrderBy(e => e.id, StringComparer.Ordinal))
            {
                entitySerialisationRegistry.AddSynchronisedType(entry.componentType, entry.config);
            }
        }
    }
}