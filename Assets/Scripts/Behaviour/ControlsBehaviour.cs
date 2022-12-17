using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsBehaviour : MonoBehaviour
{
    [SerializeField, Min(0.01f)] float _sensitivity = 1f;

    public float Sensitivity => _sensitivity * 10;

    public bool Active => GameController.GameState == GameController.State.Playing;

    private void Update()
    {
        if (Active && Input.GetMouseButton(0))
            GameController.LevelMap.Rotate(0, -Input.GetAxis("Mouse X") * Sensitivity, 0);
    }
}
