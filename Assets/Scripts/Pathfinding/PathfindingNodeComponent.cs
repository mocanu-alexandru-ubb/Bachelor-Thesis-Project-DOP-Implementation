using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct PathfindingNodeData : IComponentData
{
    public float3 nextNodePosition;
    public int nodeId;
    public int nextNodeId;
    public bool nextNodeIsHomebase;
    public bool isHomebase;
}

public class PathfindingNodeComponent : MonoBehaviour, IConvertGameObjectToEntity
{
    public PathfindingNodeComponent nextNode;
    public bool isHomebase = false;
    private static int freeNodeId = 1000;
    public int currentNodeId {  get; private set; }

    private void Awake()
    {
        currentNodeId = freeNodeId++;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
            new PathfindingNodeData
            {
                nextNodePosition = nextNode.transform.position,
                nextNodeId = nextNode.currentNodeId,
                nextNodeIsHomebase = nextNode.isHomebase,
                nodeId = currentNodeId,
                isHomebase = isHomebase
            });
    }
}
