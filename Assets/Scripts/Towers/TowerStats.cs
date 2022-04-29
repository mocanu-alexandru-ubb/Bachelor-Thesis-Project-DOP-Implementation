using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct TowerStats : IComponentData
{
    public int owner;
    public int damage;
}
