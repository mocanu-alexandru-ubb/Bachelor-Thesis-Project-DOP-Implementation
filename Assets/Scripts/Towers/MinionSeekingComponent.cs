using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct MinionSeekingComponent : IComponentData
{
    public float3 targetPosition;
    public int targetId;
}
