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

    private void Start()
    {
        UpdateMaterial();
        GameController.OnPlayerDie.AddListener(e => UpdateMaterial());

        // Необходимо использовать в случае, если материал финиша изменится на Transparent
        //GameController.OnReachFinish.AddListener(e => UpdateMaterial());
    }

    private void OnValidate() => UpdateMaterial();

    private void UpdateMaterial()
    {
        switch (State)
        {
            case SectorState.Idle:
                Renderer.sharedMaterial =
                    GameController.GameState == GameController.State.Playing ?
                    Assets.Resources.MATERIALS_SECTOR_IDLE :
                    Assets.Resources.MATERIALS_SECTOR_IDLE_BLUR;
                break;
            case SectorState.Bad:
                Renderer.sharedMaterial =
                    GameController.GameState == GameController.State.Playing ?
                    Assets.Resources.MATERIALS_SECTOR_BAD :
                    Assets.Resources.MATERIALS_SECTOR_BAD_BLUR;
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

    public void Crack(float percentage)
    {
        if (State == SectorState.Idle)
            Renderer.material.SetFloat("_Cracked", Mathf.Clamp01(percentage));
    }

    public enum SectorState
    {
        Idle = 0,
        Bad = 1,
        Finish = 2
    }
}
