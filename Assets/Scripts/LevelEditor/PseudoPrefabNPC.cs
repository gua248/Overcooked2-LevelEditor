using LevelEditorStub;
using UnityEngine;


namespace LevelEditor
{
    public class PseudoPrefabNPC : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabNPCStub NPCStub = (PseudoPrefabNPCStub)stub;
            RuntimeAnimatorController controller = PseudoPrefabManager.LoadAsset<RuntimeAnimatorController>(NPCStub.animatorControllerSO);
            Animator animator = childGameObject.GetComponent<Animator>();
            if (animator != null)
            {
                animator.runtimeAnimatorController = controller;
            }

            // HandleSpecificPrefabs
            switch (stub.pseudoPrefabSO.prefabName)
            {
                case "NPC_Walk_Anticlockwise":
                    {
                        animator = childGameObject.transform.Find("Path").GetComponent<Animator>();
                        if (animator != null)
                        {
                            animator.runtimeAnimatorController = controller;
                        }
                        break;
                    }

                case "DLC03_NPC_02":
                    {
                        int body = Random.Range(0, 3);
                        int hand = Random.Range(0, 3);
                        int scarf = Random.Range(0, 3);
                        int head = Random.Range(0, 10);

                        Transform parent = childGameObject.transform.Find("CHR_dlc3_NPC/Body");
                        foreach (Transform child in parent)
                            child.gameObject.SetActive(false);
                        parent.GetChild(body).gameObject.SetActive(true);
                        parent.GetChild(hand + 3).gameObject.SetActive(true);
                        parent.GetChild(scarf + 6).gameObject.SetActive(true);

                        parent = childGameObject.transform.Find("CHR_dlc3_NPC/Heads");
                        foreach (Transform child in parent)
                            child.gameObject.SetActive(false);
                        parent.GetChild(head).gameObject.SetActive(true);
                        break;
                    }

                case "DLC07_NPCs_Keep":
                case "DLC07_NPCs_Ghost":
                    {
                        animator = childGameObject.RequestComponentRecursive<Animator>();
                        if (animator != null)
                        {
                            animator.runtimeAnimatorController = controller;
                        }
                        int head = Random.Range(0, 10);

                        Transform parent = childGameObject.transform.Find("m_dlc07_NPCs/Mesh/Heads");
                        foreach (Transform child in parent)
                            child.gameObject.SetActive(false);
                        parent.GetChild(head).gameObject.SetActive(true);
                        break;
                    }

                case "NPC_DLC_04":
                case "NPC_DLC_05":
                case "NPC_DLC_06":
                    {
                        int body = Random.Range(0, 3);
                        int hand = Random.Range(0, 3);
                        int head = Random.Range(0, 10);

                        Transform parent = childGameObject.transform.Find("Mesh/Body");
                        foreach (Transform child in parent)
                            child.gameObject.SetActive(false);
                        parent.GetChild(body).gameObject.SetActive(true);
                        parent.GetChild(hand + 3).gameObject.SetActive(true);

                        parent = childGameObject.transform.Find("Mesh/Heads");
                        foreach (Transform child in parent)
                            child.gameObject.SetActive(false);
                        parent.GetChild(head).gameObject.SetActive(true);
                        break;
                    }

                default:
                    break;
            }
        }
    }
}