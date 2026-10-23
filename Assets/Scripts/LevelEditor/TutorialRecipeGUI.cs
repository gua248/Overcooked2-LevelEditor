using GameModes.Horde;
using LevelEditorStub;
using System.Collections;
using System.Collections.Generic;
using Team17.Online;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;
using System.Reflection;

namespace LevelEditor
{
    public class TutorialRecipeGUI : UIControllerBase
    {
        private TutorialRecipeGUIStub stub;
        private GameObject recipeUIPrefab;
        private List<HordeOrderUIController> widgets = new List<HordeOrderUIController>();
        private Animator animator;

        private void Awake()
        {
            stub = GetComponent<TutorialRecipeGUIStub>();
            recipeUIPrefab = PseudoPrefabManager.LoadAsset(stub.recipeUIPrefabSO);
            animator = GetComponent<Animator>();
            Mailbox.Client.RegisterForMessageType(MessageType.GameState, new OrderedMessageReceivedCallback(OnGameStateChanged));
        }

        private void OnDestroy()
        {
            Mailbox.Client.UnregisterForMessageType(MessageType.GameState, new OrderedMessageReceivedCallback(OnGameStateChanged));
        }

        private void OnGameStateChanged(IOnlineMultiplayerSessionUserId sessionUserId, Serialisable message)
        {
            GameStateMessage gameStateMessage = (GameStateMessage)message;
            if (gameStateMessage.m_State == GameState.StartEntities)
            {
                float totHeight = 0f;
                foreach (CustomRecipeSO recipe in stub.recipes)
                {
                    RecipeList.Entry entry = RecipeHelper.GetRecipe(recipe);
                    HordeOrderUIController hordeOrderUIController = AddElement(entry.m_order);
                    hordeOrderUIController.GetComponent<RectTransformExtension>().PixelOffset += 
                        new Vector2(80 + stub.distanceFromLeft, -totHeight);
                    Vector2 size = (Vector2)typeof(HordeOrderUIController)
                        .GetField("m_size", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(hordeOrderUIController);
                    totHeight += size.y + stub.distanceBetween;
                }
            }
            else if (gameStateMessage.m_State == GameState.InLevel)
            {
                animator.SetTrigger("TransitionIn");
            }
        }
        
        public HordeOrderUIController AddElement(OrderDefinitionNode _data)
        {
            GameObject obj = GameUtils.InstantiateUIController(recipeUIPrefab, transform as RectTransform);
            obj.GetComponent<Animator>().enabled = false;
            HordeOrderUIController uiController = obj.GetComponent<HordeOrderUIController>();
            uiController.Setup(null, _data.m_orderGuiDescription, HordeOrderUIController.Align.Left);
            uiController.SetFollowOffset((transform as RectTransform).anchorMin);

            uiController.ShouldFollow = false;
            widgets.Add(uiController);
            return uiController;
        }
    }
}