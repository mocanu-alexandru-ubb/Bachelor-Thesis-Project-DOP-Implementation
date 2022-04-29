using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct TakeDamageComponent : IComponentData
{
    public int damageToTake;
}
