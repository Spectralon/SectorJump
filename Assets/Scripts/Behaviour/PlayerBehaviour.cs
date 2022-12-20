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

    [SerializeField] private ParticleSystem TrailParticles;

    [SerializeField] private ParticleSystem WinParticles;

    [SerializeField] private ParticleSystem LoseParticles;

    private bool Boosted = false;

    private int PlatformsPassed = 0;

    private void Awake()
    {
        Unfreeze();
        if(TrailParticles == null) TryGetComponent(out TrailParticles);
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
        if (LoseParticles != null) LoseParticles.Play(false);
        GameController.OnPlayerDied(this);
    }

    public void Finish()
    {
        Freeze();
        if (WinParticles != null) WinParticles.Play(false);
        GameController.OnFinishReached(this);
    }

    public void Boost()
    {
        if (Boosted) return;
        Boosted = true;
        if (TrailParticles != null) TrailParticles.Play(false);
    }

    public void Unboost()
    {
        if (!Boosted) return;
        Boosted = false;
        if (TrailParticles != null)
        {
            TrailParticles.Stop();
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[TrailParticles.particleCount];
            if (TrailParticles.GetParticles(particles) > 0)
            {
                for (int i = 0; i < particles.Length; i++)
                {
                    particles[i].remainingLifetime = 0f;
                }
                TrailParticles.SetParticles(particles);
            }
        }
    }

    public void Freeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll;

    public void Unfreeze() => Rigidbody.constraints = RigidbodyConstraints.FreezeAll & ~RigidbodyConstraints.FreezePositionY;
}
