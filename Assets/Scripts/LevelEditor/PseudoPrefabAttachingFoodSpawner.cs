using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabAttachingFoodSpawner : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabAttachingFoodSpawnerStub spawnerStub = (PseudoPrefabAttachingFoodSpawnerStub)stub;

            TriggerAttachedSpawn triggerAttachedSpawn = childGameObject.GetComponent<TriggerAttachedSpawn>();
            triggerAttachedSpawn.m_spawnInOrder = spawnerStub.spawnInOrder;
            int num = spawnerStub.attachmentPrefabSOs.Length;
            if (spawnerStub.weights.Length < num)
                spawnerStub.weights = spawnerStub.attachmentPrefabSOs.Select(_ => 1f).ToArray();
            triggerAttachedSpawn.m_attachmentPrefabs = new TriggerAttachedSpawn.WeightedPrefab[num];
            for (int i = 0; i < num; i++)
            {
                TriggerAttachedSpawn.WeightedPrefab weightedPrefab = new TriggerAttachedSpawn.WeightedPrefab
                {
                    AttachmentPrefab = PseudoPrefabManager.LoadAsset(spawnerStub.attachmentPrefabSOs[i])
                };
                weightedPrefab.GetType()
                    .GetField("m_weight", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .SetValue(weightedPrefab, spawnerStub.weights[i]);
                triggerAttachedSpawn.m_attachmentPrefabs[i] = weightedPrefab;
            }
            if (Application.isPlaying)
            {
                SpawnableEntityCollection spawnableEntityCollection = childGameObject.GetComponent<SpawnableEntityCollection>();
                if (spawnableEntityCollection != null)
                    DestroyImmediate(spawnableEntityCollection);
                triggerAttachedSpawn.GetType()
                    .GetMethod("Awake", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
                    .Invoke(triggerAttachedSpawn, null);
            }

            TriggerTimer triggerTimer = childGameObject.GetComponent<TriggerTimer>();
            triggerTimer.m_time = spawnerStub.triggerTime;
            triggerTimer.m_triggerAtStart = spawnerStub.triggerAtStart;
        }
    }
}