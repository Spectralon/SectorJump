using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerReachFinishEvent : UnityEvent<PlayerReachFinishEvent>
{
    public PlayerBehaviour Player { get; private set; }

    public void Invoke(PlayerBehaviour player)
    {
        Player = player;
        base.Invoke(this);
    }
}