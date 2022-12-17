using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDieEvent : UnityEvent<PlayerDieEvent>
{
    public PlayerBehaviour Player { get; private set; }

    public void Invoke(PlayerBehaviour player)
    {
        Player = player;
        base.Invoke(this);
    }
}
