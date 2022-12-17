using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CameraBehaviour : MonoBehaviour
{
    private Vector3 _target;

    public Vector3 Target
    {
        get => _target + Offset;
        set 
        {
            if (!Initialized)
            {
                Offset = transform.position - value;
                Initialized = true;
            }
            _target = value;
        }
    }

    private AudioSource _backgroundAudio;

    public AudioSource BackgroundAudio
    {
        get
        {
            if (_backgroundAudio == null) TryGetComponent(out _backgroundAudio);
            return _backgroundAudio;
        }
        private set => _backgroundAudio = value;
    }

    [SerializeField, Min(0)] private float _speed = 10f;

    public float Speed
    {
        get => _speed;
        set => _speed = Mathf.Max(0, value);
    }

    [SerializeField, Range(0, 1)] private float _musicMinVolume = 0.5f;

    public float MusicMinVolume
    {
        get => _musicMinVolume;
        set => _musicMinVolume = Mathf.Clamp(value, 0, 1);
    }

    [SerializeField, Range(0, 1)] private float _volumeFadeSpeed = 0.1f;

    public float VolumeFadeSpeed
    {
        get => _volumeFadeSpeed;
        set => _volumeFadeSpeed = Mathf.Clamp(value, 0, 1);
    }

    private float TargetVolume = 1f;

    private Vector3 Offset;

    private bool Initialized = false;

    private void Start()
    {
        GameController.OnPlayerDie.AddListener(e => TargetVolume = MusicMinVolume);
        GameController.OnReachFinish.AddListener(e => TargetVolume = MusicMinVolume);
    }

    void Update()
    {
        if (!Initialized) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            Target,
            Speed * Time.deltaTime
        );

        if (TargetVolume >= 1 || BackgroundAudio.volume <= MusicMinVolume) return;

        BackgroundAudio.volume = Mathf.MoveTowards(BackgroundAudio.volume, TargetVolume, VolumeFadeSpeed * Time.deltaTime);
    }
}
