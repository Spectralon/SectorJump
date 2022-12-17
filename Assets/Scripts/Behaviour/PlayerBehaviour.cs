using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI = UIController;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]
public class PlayerBehaviour : MonoBehaviour
{
    public Vector3 BounceSpeed = new(0, 12, 0);

    public PlatformBehaviour CurrentPlatform { get; set; }

    public float TargetY { get; set; } = 0;

    private float _minReachedY;

    public float MinReachedY 
    { 
        get => _minReachedY;
        set 
        {
            if (value < _minReachedY && TargetY != 0)
            {
                _minReachedY = value;
                GameController.GameCamera.Target = transform.position;
                UI.UISetProgress(_minReachedY / TargetY);
            }
        }
    }

    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody
    {
        get
        {
            if (_rigidbody == null) TryGetComponent(out _rigidbody);
            return _rigidbody;
        }
        private set => _rigidbody = value;
    }

    private AudioSource _bounceAudio;

    public AudioSource BounceAudio
    {
        get
        {
            if (_bounceAudio == null) TryGetComponent(out _bounceAudio);
            return _bounceAudio;
        }
        private set => _bounceAudio = value;
    }

    private void Awake() => Unfreeze();

    private void Start() => GameController.OnContactSector.AddListener(e => e.Player.BounceAudio.Play());

    private void Update() => MinReachedY = transform.position.y;

    public void Bounce() => Rigidbody.velocity = BounceSpeed;

    public void Die()
    {
        Freeze();
        GameController.OnPlayerDied(this);
    }

    public void Finish()
    {
        Freeze();
        GameController.OnFinishReached(this);
    }

    public void Freeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

    public void Unfreeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionY;
}
