using System.Collections;
using System.Collections.Generic;
using Team17.Online.Multiplayer.Messaging;
using UnityEngine;

namespace LevelEditor
{
    public interface IModEntityClient
    {
        Serialisable GetSerialisable();
    }
}