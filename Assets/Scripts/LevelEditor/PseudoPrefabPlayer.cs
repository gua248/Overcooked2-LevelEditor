using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelEditorStub;


namespace LevelEditor
{
    public class PseudoPrefabPlayer : PseudoPrefab
    {
        public override void Setup()
        {
            PseudoPrefabPlayerStub playerStub = (PseudoPrefabPlayerStub)stub;
            PlayerIDProvider playerIDProvider = childGameObject.GetComponent<PlayerIDProvider>();
            playerIDProvider.OverridePlayerId((PlayerInputLookup.Player)playerStub.playerID);
        }
    }
}
