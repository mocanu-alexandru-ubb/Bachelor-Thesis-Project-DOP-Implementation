using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct ClickableTileData : IComponentData
{
    public bool isHighlited;
    public bool materialNeedsChange;
}

[DisallowMultipleComponent]
public class ClickableTileComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ClickableTileData
        {
            isHighlited = false,
            materialNeedsChange = false
        });
    }
}
