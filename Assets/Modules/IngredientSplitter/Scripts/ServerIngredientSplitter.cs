using LevelEditor;
using System.Collections;
using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;
using OC2DIYLevel.Modules.IngredientSplitterMod;

namespace IngredientSplitterMod
{
    public class ServerIngredientSplitter : ServerSynchroniserBase, IHandlePlacement, IHandlePickup
    {
        private IngredientSplitter splitter;
        private AssembledDefinitionNode inputAssembledNode;
        private AssembledDefinitionNode outputAssembledNode;
        private int ingredientCnt;
        private IngredientSplitterMessage message = new IngredientSplitterMessage();

        public override void StartSynchronising(Component synchronisedObject)
        {
            base.StartSynchronising(synchronisedObject);
            splitter = (IngredientSplitter)synchronisedObject;
            NetworkUtils.RegisterSpawnablePrefab(gameObject, splitter.outputPrefab);
            inputAssembledNode = RecipeHelper.GetOrderDefinitionNodeCustomRecipe(splitter.inputRecipe).Simpilfy();
            outputAssembledNode = RecipeHelper.GetIngredientOrderNode(splitter.outputPrefab).Simpilfy();
            ingredientCnt = 0;
        }

        public bool CanHandlePickup(ICarrier _carrier)
        {
            return ingredientCnt > 0;
        }

        public void HandlePickup(ICarrier _carrier, Vector2 _directionXZ)
        {
            Vector3 position = gameObject.transform.position;
            IParentable parentable = _carrier.AccessGameObject().RequestInterface<IParentable>();
            if (parentable as MonoBehaviour != null)
            {
                position = parentable.GetAttachPoint(splitter.outputPrefab).position;
            }
            GameObject output = NetworkUtils.ServerSpawnPrefab(gameObject, splitter.outputPrefab, position, Quaternion.identity);
            _carrier.CarryItem(output);
            ingredientCnt--;
            SendPickup();
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
            IOrderDefinition orderDefinition = carried.GetComponent<IOrderDefinition>();
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
            IOrderDefinition orderDefinition = carried.GetComponent<IOrderDefinition>();
            if (orderDefinition == null) return false;

            AssembledDefinitionNode assembledDefinitionNode = orderDefinition.GetOrderComposition().Simpilfy();
            return AssembledDefinitionNode.MatchingAlreadySimple(assembledDefinitionNode, inputAssembledNode);
        }

        public void HandlePlacement(ICarrier _carrier, Vector2 _directionXZ, PlacementContext _context)
        {
            GameObject carried = _carrier.InspectCarriedItem();

            if (MatchOutput(carried))
            {
                GameObject gameObject = _carrier.TakeItem();
                NetworkUtils.DestroyObject(gameObject);
                ingredientCnt += 1;
                SendPlacement();
            }
            else if (MatchInput(carried))
            {
                ServerIngredientContainer serverIngredientContainer = carried.GetComponent<ServerIngredientContainer>();
                ServerPreparationContainer serverPreparationContainer = carried.GetComponent<ServerPreparationContainer>();
                if (serverIngredientContainer != null && serverPreparationContainer == null)
                {
                    serverIngredientContainer.Empty();
                }
                else
                {
                    GameObject gameObject = _carrier.TakeItem();
                    NetworkUtils.DestroyObject(gameObject);
                }
                ingredientCnt += splitter.splitNum;
                SendPlacement();
            }
        }

        public void OnFailedToPlace(GameObject _item)
        {
        }

        public int GetPlacementPriority()
        {
            return 1;
        }

        private void SendPickup()
        {
            message.messageType = IngredientSplitterMessage.MessageType.Take;
            message.ingredientCnt = ingredientCnt;
            SendServerEvent(message);
        }

        private void SendPlacement()
        {
            message.messageType = IngredientSplitterMessage.MessageType.Add;
            message.ingredientCnt = ingredientCnt;
            SendServerEvent(message);
        }
    }
}