using BitStream;
using System.Collections;
using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

namespace IngredientSplitterMod
{
    public class IngredientSplitterMessage : Serialisable
    {
        public int ingredientCnt;
        public MessageType messageType;
        private const int ingredientCntBits = 6;
        private const int messageTypeBits = 1;

        public enum MessageType
        {
            Add,
            Take,
        }

        public void Serialise(BitStreamWriter writer)
        {
            writer.Write((uint)messageType, messageTypeBits);
            writer.Write((uint)ingredientCnt, ingredientCntBits);
        }

        public bool Deserialise(BitStreamReader reader)
        {
            messageType = (MessageType)reader.ReadUInt32(messageTypeBits);
            ingredientCnt = (int)reader.ReadUInt32(ingredientCntBits);
            return true;
        }
    }
}