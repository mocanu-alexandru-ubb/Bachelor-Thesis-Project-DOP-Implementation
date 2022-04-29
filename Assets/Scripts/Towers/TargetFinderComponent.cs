using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct TargetFinderComponent : IComponentData
{
    public float towerRange;
}
