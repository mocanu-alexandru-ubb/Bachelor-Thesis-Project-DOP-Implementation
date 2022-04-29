using System;
using Unity.Entities;

[Serializable]
[GenerateAuthoringComponent]
public struct FireRateComponent : IComponentData
{
    public int rateOfFire;
    public float cooldown;
}
