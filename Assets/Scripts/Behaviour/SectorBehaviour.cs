using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SectorBehaviour : MonoBehaviour
{
    private Renderer _renderer;

    public Renderer Renderer
    {
        get
        {
            if (_renderer == null) TryGetComponent(out _renderer);
            return _renderer;
        }
        private set => _renderer = value;
    }

    [SerializeField] SectorState _state;

    public SectorState State
    {
        get => _state;
        set
        {
            _state = value;
            UpdateMaterial();
        }
    }

    private void Awake() => UpdateMaterial();

    private void OnValidate() => UpdateMaterial();

    private void UpdateMaterial()
    {
        switch (State)
        {
            case SectorState.Idle:
                Renderer.sharedMaterial = Assets.Resources.MATERIALS_SECTOR_IDLE;
                break;
            case SectorState.Bad:
                Renderer.sharedMaterial = Assets.Resources.MATERIALS_SECTOR_BAD;
                break;
            case SectorState.Finish:
                Renderer.sharedMaterial = Assets.Resources.MATERIALS_SECTOR_FINISH;
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.TryGetComponent(out PlayerBehaviour player)) return;
        GameController.OnSectorTouched(this, player);

        switch (State)
        {
            case SectorState.Idle:
                player.Bounce();
                break;
            case SectorState.Bad:
                player.Die();
                break;
            case SectorState.Finish:
                player.Finish();
                break;
            default:
                break;
        }
    }

    public enum SectorState
    {
        Idle = 0,
        Bad = 1,
        Finish = 2
    }
}
