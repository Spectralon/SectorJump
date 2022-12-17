using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlatformBehaviour : MonoBehaviour
{
    public int Score;

    [SerializeField, Range(0, 10)] float FadeSpeed;

    [SerializeField, Min(0)] float BreakForce;

    [SerializeField, Min(0)] float BreakStrength;

    private readonly Queue<Renderer> ToDestroy = new();

    private AudioSource _breakAudio;

    public AudioSource BreakAudio
    {
        get
        {
            if (_breakAudio == null) TryGetComponent(out _breakAudio);
            return _breakAudio;
        }
        private set => _breakAudio = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out PlayerBehaviour player)) return;

        if (player.CurrentPlatform == null)
        {
            player.CurrentPlatform = this;
            return;
        }

        if (player.CurrentPlatform.transform.position.y <= transform.position.y) return;

        GameController.OnPlatformEntered(this, player);

        player.CurrentPlatform.Break();

        player.CurrentPlatform = this;
    }

    public void Damage(float damage)
    {
        BreakStrength -= damage;

        if (BreakStrength <= 0) Break();
    }

    private void Break()
    {
        BreakAudio.Play();
        foreach (var sector in GetComponentsInChildren<SectorBehaviour>())
        {
            GameObject child = sector.gameObject;

            // Периодически такой баг происходит. Скорее всего - из-за частичного проникновения игрока в сектор финиша при падении с большой высоты
            // (платформа считается пройденной и ломается). Но я это не проверял т.к. его очень сложно поймать. В рамках ДЗ приемлем и такой фикс.
            if (sector.State == SectorBehaviour.SectorState.Finish) return;

            if (!child.TryGetComponent(out Rigidbody rigidbody)) 
                rigidbody = child.AddComponent<Rigidbody>();
            else
                rigidbody.constraints = RigidbodyConstraints.None;

            rigidbody.useGravity = true;
            rigidbody.AddForce((rigidbody.worldCenterOfMass - transform.position) * BreakForce, ForceMode.Impulse);

            ToDestroy.Enqueue(sector.Renderer);
        }
    }

    private void Update()
    {
        int left = ToDestroy.Count;
        while (left-- > 0)
        {
            Renderer item = ToDestroy.Dequeue();
            if (item == null) continue;

            Color color = item.material.color;
            color.a -= FadeSpeed * Time.deltaTime;
            item.material.color = color;

            if (color.a <= 0) Destroy(item.gameObject);
            else ToDestroy.Enqueue(item);
        }
    }
}
