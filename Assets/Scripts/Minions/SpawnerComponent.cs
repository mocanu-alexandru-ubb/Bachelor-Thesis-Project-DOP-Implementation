using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct SpawnerComponentData : IComponentData
{
    public float spawnsPerSecond;
    public float spawnCooldown;
    public float3 spawnPosition;
    public PathfindingTargetData targetNode;
}

[DisallowMultipleComponent]
public class SpawnerComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public PathfindingNodeComponent initialEnemy;
    public float spawnsPerSecond = 1f;
    public float spawnCooldown = 0f;

    private void Awake()
    {
        initialEnemy = GetComponent<PathfindingNodeComponent>();
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SpawnerComponentData
        {
            spawnCooldown = spawnCooldown,
            spawnsPerSecond = spawnsPerSecond,
            spawnPosition = transform.position,
            targetNode = new PathfindingTargetData
            {
                nodePosition = transform.position,
                nodeId = initialEnemy.currentNodeId,
                nodeIsHomebase = false
            }
        });
        dstManager.AddBuffer<MinionSpawnQueue>(entity);
    }
}
