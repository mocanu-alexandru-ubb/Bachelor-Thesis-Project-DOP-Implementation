using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct BulletSpawningPoint : IComponentData
{
    public float3 spawnPosition;
}

[DisallowMultipleComponent]
public class BulletSpawningPointComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject spawningPoint;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BulletSpawningPoint { spawnPosition = transform.position });
    }
}

