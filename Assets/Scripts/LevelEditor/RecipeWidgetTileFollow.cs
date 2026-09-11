using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelEditor
{
    public class RecipeWidgetTileFollow : MonoBehaviour
    {
        private RecipeWidgetUIController recipeWidgetUIController;
        private List<RecipeWidgetTile> tiles;
        private Animator animator;
        private static readonly int NothingHash = Animator.StringToHash("Nothing");

        private void Awake()
        {
            recipeWidgetUIController = GetComponent<RecipeWidgetUIController>();
        }

        public void SetTiles(List<RecipeWidgetTile> tiles)
        {
            this.tiles = tiles;
        }

        public void SetAnimator(Animator animator)
        {
            this.animator = animator;
        }

        private void LateUpdate()
        {
            CopyRect();
            var state = animator.GetCurrentAnimatorStateInfo(1);
            if (state.shortNameHash == NothingHash && !animator.IsInTransition(1))
            {
                CopyRect();
                enabled = false;
            }
        }

        private void CopyRect()
        {
            if (tiles != null && tiles.Count > 5)
            {
                RectTransform rectTransform0 = tiles[0].transform.GetChild(0) as RectTransform;
                RectTransform rectTransform00 = rectTransform0.GetChild(0) as RectTransform;
                RectTransform rectTransform01 = rectTransform0.GetChild(1) as RectTransform;

                for (int i = 4; i < tiles.Count - 1; i++)
                {
                    RectTransform rectTransformi = tiles[i].transform.GetChild(0) as RectTransform;
                    RectTransform rectTransformi0 = rectTransformi.GetChild(0) as RectTransform;
                    RectTransform rectTransformi1 = rectTransformi.GetChild(1) as RectTransform;
                    rectTransformi.anchoredPosition = rectTransform0.anchoredPosition;
                    rectTransformi.sizeDelta = rectTransform0.sizeDelta;
                    rectTransformi0.anchoredPosition = rectTransform00.anchoredPosition;
                    rectTransformi0.sizeDelta = rectTransform00.sizeDelta;
                    rectTransformi1.anchoredPosition = rectTransform01.anchoredPosition;
                    rectTransformi1.sizeDelta = rectTransform01.sizeDelta;
                }
            }
        }
    }
}