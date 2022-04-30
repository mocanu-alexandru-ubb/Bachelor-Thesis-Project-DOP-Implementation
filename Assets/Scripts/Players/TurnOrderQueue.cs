using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[InternalBufferCapacity(16)]
public struct TurnOrderQueue : IBufferElementData
{
    public Entity Value;
}

[DisallowMultipleComponent]
public class TurnOrderQueueAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<TurnOrderQueue>(entity);
    }
}
