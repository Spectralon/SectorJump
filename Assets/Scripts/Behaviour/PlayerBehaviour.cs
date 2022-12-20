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

    private ParticleSystem ParticleSystem;

    private bool Boosted = false;

    private int PlatformsPassed = 0;

    private void Awake()
    {
        Unfreeze();
        TryGetComponent(out ParticleSystem);
    }

    private void Start()
    {
        GameController.OnContactSector.AddListener(e =>
        {
            PlatformsPassed = 0;
            e.Player.BounceAudio.Play();
            e.Player.Unboost();
        });
        GameController.OnEnterPlatform.AddListener(e =>
        {
            if (++e.Player.PlatformsPassed > 1) e.Player.Boost();
        });
    }

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

    public void Boost()
    {
        if (Boosted) return;
        Boosted = true;
        if (ParticleSystem != null) ParticleSystem.Play();
    }

    public void Unboost()
    {
        if (!Boosted) return;
        Boosted = false;
        if (ParticleSystem != null)
        {
            ParticleSystem.Stop();
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ParticleSystem.particleCount];
            if (ParticleSystem.GetParticles(particles) > 0)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].remainingLifetime = 0f;
                }
                ParticleSystem.SetParticles(particles);
            }
        }
    }

    public void Freeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

    public void Unfreeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionY;
}
