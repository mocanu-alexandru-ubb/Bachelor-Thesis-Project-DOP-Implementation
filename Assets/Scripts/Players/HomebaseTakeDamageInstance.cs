using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct HomebaseTakeDamageInstance : IComponentData
{
    public int nodeId;
    public int damageToTake;
}
