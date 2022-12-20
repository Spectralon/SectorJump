using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "HelixJump/Assets reference")]
public class Assets : ScriptableObject
{
    private const string ResourcesKey = "RecourceReference";

    public Material MATERIALS_SECTOR_IDLE;

    public Material MATERIALS_SECTOR_BAD;

    public Material MATERIALS_SECTOR_FINISH;

    public Material MATERIALS_SECTOR_IDLE_BLUR;

    public Material MATERIALS_SECTOR_BAD_BLUR;

    private static Assets _reference;

    public static Assets Resources {
        get
        {
            _reference ??= UnityEngine.Resources.Load<Assets>(ResourcesKey);   // Перед присваиванием пройдёт проверка на нулл. Без необходимости ресурс загружаться не будет.

            return _reference;
        }
        private set => _reference ??= value;    // Здесь и выше применение оператора ??= корректно, несмотря на предупреждение Unity.
    }

    private void Awake() => Resources = this;

    private void OnValidate() => Resources = this;
}
