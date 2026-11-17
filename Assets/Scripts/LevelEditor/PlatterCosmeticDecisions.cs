using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class PlatterCosmeticDecisions : SaladCosmeticDecisions
    {
        public CustomRecipeOptionalPlatterSO.CosmeticsType cosmeticsType = CustomRecipeOptionalPlatterSO.CosmeticsType.Overlap;
        public Vector3 offset = Vector3.zero;

        public void Init(CustomRecipeOptionalPlatterSO platterSO)
        {
            cosmeticsType = platterSO.cosmeticsType;
            offset = platterSO.offset;
        }

        protected override void CreateRenderChildren(AssembledDefinitionNode _contents)
        {
            if (cosmeticsType == CustomRecipeOptionalPlatterSO.CosmeticsType.Overlap)
            {
                base.CreateRenderChildren(_contents);
                return;
            }

            IEnumerator<AssembledDefinitionNode> enumerator = ShallowIteration(_contents);
            List<AssembledDefinitionNode> list = new List<AssembledDefinitionNode>();
            while (enumerator.MoveNext())
                list.Add(enumerator.Current);
            Quaternion quaternion = Quaternion.Euler(0f, -360f / (float)list.Count, 0f);
            Vector3 vector = offset;
            Quaternion quaternion2 = Quaternion.identity;
            for (int i = 0; i < list.Count; i++)
            {
                AssembledDefinitionNode order = list[i];
                GameObject renderPefabForNode = GetRenderPefabForNode(order);
                if (renderPefabForNode != null)
                {
                    GameObject gameObject = renderPefabForNode.InstantiateOnParent(m_container.transform, true);
                    if (list.Count <= 1) continue;
                    gameObject.transform.localPosition = vector;
                    gameObject.transform.localRotation = quaternion2;
                    vector = quaternion * vector;
                    quaternion2 = quaternion * quaternion2;
                }
            }
        }
    }
}