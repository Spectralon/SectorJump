using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour
{
    [SerializeField] private PlatformBehaviour _finishPlatform;

    public PlatformBehaviour FinishPlatform
    {
        get => _finishPlatform;
        set
        {
            if (value == null)
            {
                Debug.LogWarning("Finish Platform must not be null!");
                return;
            }

            _finishPlatform = value;
        }
    }

    [SerializeField, Min(0)] private float _difficulty = 2;

    public float Difficulty
    {
        get => _difficulty;
        set
        {
            _difficulty = Mathf.Clamp(value, 0, 10);
        }
    }

    public PlatformBehaviour[] Platforms;

    public PlatformBehaviour StartPlatform;

    public Transform Rod;

    [SerializeField, Min(2)] protected int MinPlatforms = 3;

    [SerializeField, Min(2)] protected int MaxPlatforms = 5;

    [SerializeField, Min(1)] protected float Interval = 1f;

    [SerializeField, Min(0)] protected float RodScaleOffset = 1f;

    private readonly List<PlatformBehaviour> ActivePlatforms = new();

    public void Init()
    {
        int count = GetCount();

        AddPlatform(StartPlatform.gameObject, true).TryGetComponent(out StartPlatform);

        if (Platforms.Length > 0)
        {
            for (int i = 0; i < count - 2; i++)
                AddPlatform(Platforms[GameController.Random.Next(0, Platforms.Length)].gameObject);
        }

        FinishPlatform = AddPlatform(FinishPlatform.gameObject, true).GetComponent<PlatformBehaviour>();

        Rod.localScale = new Vector3(
            Rod.localScale.x,
            count / 2f + RodScaleOffset,
            Rod.localScale.z
            );
    }

    private GameObject AddPlatform(GameObject prefab, bool blockRotation = false)
    {
        Transform platform = Instantiate(prefab, GameController.LevelMap).transform;

        platform.localPosition = new Vector3(0, -Interval * ActivePlatforms.Count, 0);
        if(!blockRotation) platform.localRotation = Quaternion.Euler(0, GameController.Random.Next(0, 360), 0);

        ActivePlatforms.Add(platform.gameObject.GetComponent<PlatformBehaviour>());

        return platform.gameObject;
    }

    private int GetCount()
    {
        // Нижняя граница количества платформ сдвигается вверх в зависимости от роста уровня со скоростью, зависящей от сложности. Предел - верхняя граница.
        // Если сложность 0 - не сдвигается.
        int minPlatforms = Mathf.RoundToInt(MaxPlatforms - (MaxPlatforms - MinPlatforms) / Mathf.Pow(GameController.Level + 1, Difficulty / 10f));
        
        int res = GameController.Random.Next(Mathf.Min(minPlatforms, MaxPlatforms), MaxPlatforms + 1);

        return Mathf.Max(res, 2);
    }
}
