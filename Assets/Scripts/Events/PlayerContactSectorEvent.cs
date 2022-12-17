using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

public class PlayerContactSectorEvent : UnityEvent<PlayerContactSectorEvent>
{
    public SectorBehaviour Sector { get; private set; }

    public PlayerBehaviour Player { get; private set; }

    public void Invoke(SectorBehaviour sector, PlayerBehaviour player)
    {
        Sector = sector;
        Player = player;
        base.Invoke(this);
    }
}
