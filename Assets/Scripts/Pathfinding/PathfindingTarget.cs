using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


public struct PathfindingTargetData : IComponentData
{
    public float3 nodePosition;
    public int nodeId;
    public bool nodeIsHomebase;
}


[DisallowMultipleComponent]
public class PathfindingTarget : MonoBehaviour, IConvertGameObjectToEntity
{
    public PathfindingNodeComponent targetNode;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new PathfindingTargetData { 
            nodePosition = targetNode.transform.position, 
            nodeId = targetNode.currentNodeId, 
            nodeIsHomebase = targetNode.isHomebase
        });
    }
}
