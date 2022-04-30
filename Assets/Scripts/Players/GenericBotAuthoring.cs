using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct GenericBotTag : IComponentData
{
}

[Serializable]
[InternalBufferCapacity(8)]
public struct ChooseEnemyRandomlyList : IBufferElementData
{
    public PathfindingTargetData Value;
}

[DisallowMultipleComponent]
public class GenericBotAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<PathfindingNodeComponent> possibleEnemies;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<GenericBotTag>(entity);
        dstManager.AddBuffer<ChooseEnemyRandomlyList>(entity);

        NativeArray<ChooseEnemyRandomlyList> possibleNodeTargets = new NativeArray<ChooseEnemyRandomlyList>(possibleEnemies.Count, Allocator.Temp);
        int counter = 0;
        possibleEnemies.ForEach((enemy) =>
        {
            possibleNodeTargets[counter++] = new ChooseEnemyRandomlyList
            {
                Value = new PathfindingTargetData
                {
                    nodeId = enemy.currentNodeId,
                    nodeIsHomebase = enemy.isHomebase,
                    nodePosition = enemy.transform.position
                }
            };
        });
        dstManager.GetBuffer<ChooseEnemyRandomlyList>(entity).AddRange(possibleNodeTargets);
    }
}