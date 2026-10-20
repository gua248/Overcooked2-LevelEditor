using LevelEditor;
using System.Collections;
using System.Collections.Generic;
using Team17.Online;
using Team17.Online.Multiplayer.Messaging;
using UnityEditor;
using UnityEngine;
using MessageType = Team17.Online.Multiplayer.Messaging.MessageType;
using OC2DIYLevel.Modules.IngredientSplitterMod;

namespace IngredientSplitterMod
{
    public class ClientIngredientSplitter : ClientSynchroniserBase, IClientHandlePickup, IClientHandlePlacement, IModEntityClient
    {
        private IngredientSplitter splitter;
        private AssembledDefinitionNode inputAssembledNode;
        private AssembledDefinitionNode outputAssembledNode;
        private int ingredientCnt;
        private IngredientSplitterMessage message = new IngredientSplitterMessage();

        private ButtonHoverIcon tutorialUI;
        private List<ClientPlayerAttachmentCarrier> localPlayerCarriers;

        public Serialisable GetSerialisable()
        {
            return message;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
#endif
        public static void RegisterModule()
        {
            ModuleManager.Register(
                "dev.gua.overcooked.diylevel.ingredientsplittermod",
                typeof(IngredientSplitter),
                new SynchroniserConfig(InstancesPerGameObject.Single, typeof(ServerIngredientSplitter), typeof(ClientIngredientSplitter)));
        }
        
        private void Awake()
        {
            tutorialUI = GetComponent<ButtonHoverIcon>();
            localPlayerCarriers = new List<ClientPlayerAttachmentCarrier>();
        }

        public override void StartSynchronising(Component synchronisedObject)
        {
            base.StartSynchronising(synchronisedObject);
            splitter = (IngredientSplitter)synchronisedObject;
            NetworkUtils.RegisterSpawnablePrefab(gameObject, splitter.outputPrefab);
            inputAssembledNode = RecipeHelper.GetOrderDefinitionNodeCustomRecipe(splitter.inputRecipe).Simpilfy();
            outputAssembledNode = RecipeHelper.GetIngredientOrderNode(splitter.outputPrefab).Simpilfy();
            ingredientCnt = 0;
            UpdateVisuals();

            Mailbox.Client.RegisterForMessageType(MessageType.GameState, new OrderedMessageReceivedCallback(OnGameStateChanged));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (ICarrier carrier in localPlayerCarriers)
            {
                if (carrier != null)
                    carrier.UnregisterCarriedItemChangeCallback(new VoidGeneric<GameObject, GameObject>(UpdateTutorialVisibility));
            }
            localPlayerCarriers.Clear();
            Mailbox.Client.UnregisterForMessageType(MessageType.GameState, new OrderedMessageReceivedCallback(OnGameStateChanged));
        }

        private void OnGameStateChanged(IOnlineMultiplayerSessionUserId sessionUserId, Serialisable message)
        {
            GameStateMessage gameStateMessage = (GameStateMessage)message;
            if (gameStateMessage.m_State == GameState.StartEntities && tutorialUI != null)
            {
                for (int i = 0; i < PlayerIDProvider.s_AllProviders.Count; i++)
                {
                    PlayerIDProvider playerIDProvider = PlayerIDProvider.s_AllProviders._items[i];
                    if (playerIDProvider != null && playerIDProvider.IsLocallyControlled())
                    {
                        ClientPlayerAttachmentCarrier carrier = playerIDProvider.GetComponent<ClientPlayerAttachmentCarrier>();
                        if (carrier != null)
                        {
                            localPlayerCarriers.Add(carrier);
                            carrier.RegisterCarriedItemChangeCallback(new VoidGeneric<GameObject, GameObject>(UpdateTutorialVisibility));
                        }
                    }
                }
            }
        }

        public override void ApplyServerEvent(Serialisable serialisable)
        {
            IngredientSplitterMessage ingredientSplitterMessage = (IngredientSplitterMessage)serialisable;
            ingredientCnt = ingredientSplitterMessage.ingredientCnt;
            //Debug.Log("Message: " + ingredientCnt.ToString());
            UpdateVisuals();
            if (ingredientSplitterMessage.messageType == IngredientSplitterMessage.MessageType.Add)
            {
                UpdateTutorialVisibility(null, null);
                GameOneShotAudioTag tag = (GameOneShotAudioTag)splitter.splitAudioTag;
                GameUtils.TriggerAudio(tag, gameObject.layer);
            }
            else
            {
            }
        }

        public bool CanHandlePickup(ICarrier _carrier)
        {
            return ingredientCnt > 0;
        }

        public int GetPickupPriority()
        {
            return 1;
        }

        public bool CanHandlePlacement(ICarrier _carrier, Vector2 _directionXZ, PlacementContext _context)
        {
            GameObject carried = _carrier.InspectCarriedItem();
            if (MatchInput(carried))
                return ingredientCnt + splitter.splitNum <= splitter.capacity;
            else if (MatchOutput(carried))
                return ingredientCnt < splitter.capacity;
            return false;
        }

        public bool MatchOutput(GameObject carried)
        {
            if (carried == null) return false;
            IClientOrderDefinition orderDefinition = carried.GetComponent<IClientOrderDefinition>();
            if (orderDefinition == null) return false;

            AssembledDefinitionNode assembledDefinitionNode = orderDefinition.GetOrderComposition().Simpilfy();
            if (AssembledDefinitionNode.MatchingAlreadySimple(assembledDefinitionNode, outputAssembledNode))
            {
                IngredientContainer ingredientContainer = carried.GetComponent<IngredientContainer>();
                PreparationContainer preparationContainer = carried.GetComponent<PreparationContainer>();
                if (ingredientContainer != null && preparationContainer == null)
                    return false;
                bool flag1 = carried.GetComponent<WorkableItem>() != null;
                bool flag2 = splitter.outputPrefab.GetComponent<WorkableItem>() != null;
                return flag1 == flag2;
            }
            return false;
        }

        public bool MatchInput(GameObject carried)
        {
            if (carried == null) return false;
            IClientOrderDefinition orderDefinition = carried.GetComponent<IClientOrderDefinition>();
            if (orderDefinition == null) return false;

            AssembledDefinitionNode assembledDefinitionNode = orderDefinition.GetOrderComposition().Simpilfy();
            return AssembledDefinitionNode.MatchingAlreadySimple(assembledDefinitionNode, inputAssembledNode);
        }

        public int GetPlacementPriority()
        {
            return 1;
        }

        private void UpdateVisuals()
        {
            if (!splitter.ingredientModels.IsEmpty())
            {
                foreach (var model in splitter.ingredientModels)
                    model.gameObject.SetActive(false);
                int n = splitter.ingredientModels.Length;
                if (ingredientCnt >= splitter.capacity)
                {
                    splitter.ingredientModels[n - 1].gameObject.SetActive(true);
                }
                else if (ingredientCnt > 0)
                {
                    int k = Mathf.CeilToInt(MathUtils.ClampedRemap(ingredientCnt, 0, splitter.capacity - 1, -1, n - 2) - 0.001f);
                    if (k >= 0)
                        splitter.ingredientModels[k].gameObject.SetActive(true);
                }
            }
        }

        private void UpdateTutorialVisibility(GameObject before, GameObject after)
        {
            foreach (ClientPlayerAttachmentCarrier carrier in localPlayerCarriers)
            {
                if (carrier != null)
                {
                    GameObject carried = carrier.InspectCarriedItem();
                    if (before == carried && after == null) continue;
                    if (MatchInput(carried))
                    {
                        tutorialUI.SetVisibility(true);
                        return;
                    }
                }
            }
            tutorialUI.SetVisibility(false);
        }
    }
}