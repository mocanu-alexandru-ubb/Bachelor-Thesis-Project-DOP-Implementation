using System;
using Unity.Entities;
using Unity.Transforms;

[Serializable]
[GenerateAuthoringComponent]
public struct BasicBulletFollowComponent : IComponentData
{
    public int targetId;
}
