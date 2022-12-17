using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerEnterPlatformEvent : UnityEvent<PlayerEnterPlatformEvent>
{
    public PlatformBehaviour Platform { get; private set; }

    public PlayerBehaviour Player { get; private set; }

    public void Invoke(PlatformBehaviour platform, PlayerBehaviour player)
    {
        Platform = platform;
        Player = player;
        Invoke(this);
    }
}
