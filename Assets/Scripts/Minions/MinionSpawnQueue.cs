using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


[InternalBufferCapacity(16)]
public struct MinionSpawnQueue : IBufferElementData
{
    public Entity Value;
}
