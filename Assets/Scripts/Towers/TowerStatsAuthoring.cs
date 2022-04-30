using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct TowerStats : IComponentData
{
    public int owner;
    public int damage;
}

[DisallowMultipleComponent]
public class TowerStatsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public PathfindingNodeComponent ownerNode;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TowerStats
        {
            owner = ownerNode.currentNodeId,
            damage = 1
        });
    }
}