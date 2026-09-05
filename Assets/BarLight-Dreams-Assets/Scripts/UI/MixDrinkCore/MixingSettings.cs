using System;
using UnityEngine;

[Serializable]
public class MixingSettings
{
    [Min(1f)]
    public float requiredShakeDistance = 1000f;
}
