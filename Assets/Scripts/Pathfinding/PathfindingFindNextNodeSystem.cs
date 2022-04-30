using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
[UpdateBefore(typeof(VelocitySystem))]
public partial class PathfindingFindNextNodeSystem : SystemBase
{
    private EntityQuery dataCollectionQuerry;

    private struct PathfindingNodeMapData
    {
        public Entity nodeEntity;
        public PathfindingNodeData nodeData;
        public int totalDamageToTake;
    }

    protected override void OnUpdate()
    {
        int entitiesInQuery = dataCollectionQuerry.CalculateEntityCount();
        NativeHashMap<int, PathfindingNodeMapData> pathfindingNodesMap = new NativeHashMap<int, PathfindingNodeMapData>(entitiesInQuery, Allocator.TempJob);

        Entities
            .WithStoreEntityQueryInField(ref dataCollectionQuerry)
            .ForEach((Entity self, in PathfindingNodeData node) =>
            {
                pathfindingNodesMap.Add(node.nodeId, new PathfindingNodeMapData { nodeEntity = self, nodeData = node});
            }).Run();

        EntityCommandBufferSystem ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer.ParallelWriter parallelEcb = ecbs.CreateCommandBuffer().AsParallelWriter();
        var finalJobHandle = Entities
            .WithReadOnly(pathfindingNodesMap)
            .WithDisposeOnCompletion(pathfindingNodesMap)
            .WithDisposeOnCompletion(parallelEcb)
            .ForEach((int entityInQueryIndex, Entity self, ref PathfindingTargetData targetNode, ref VelocityComponent velocityComponent, in LocalToWorld transform, in MinionComponent minionStats) =>
            {
                if (math.distancesq(transform.Position, targetNode.nodePosition) < 0.01f)
                {
                    if (targetNode.nodeIsHomebase)
                    {
                        parallelEcb.DestroyEntity(entityInQueryIndex, self);
                        Entity damageInstance = parallelEcb.CreateEntity(entityInQueryIndex);
                        parallelEcb.AddComponent(entityInQueryIndex, damageInstance, new HomebaseTakeDamageInstance { nodeId = targetNode.nodeId, damageToTake = minionStats.damage });
                    }

                    var currentTargetNodeData = pathfindingNodesMap[targetNode.nodeId].nodeData;
                    targetNode = new PathfindingTargetData
                    {
                        nodePosition = currentTargetNodeData.nextNodePosition,
                        nodeIsHomebase = currentTargetNodeData.nextNodeIsHomebase,
                        nodeId = currentTargetNodeData.nextNodeId
                    };

                    float3 direction = float3.zero;
                    float diffX = targetNode.nodePosition.x - transform.Position.x;
                    float diffZ = targetNode.nodePosition.z - transform.Position.z;
                    direction.x = math.abs(diffX) < 0.2 ? 0 : diffX / math.abs(diffX);
                    direction.z = math.abs(diffZ) < 0.2 ? 0 : diffZ / math.abs(diffZ);


                    velocityComponent = new VelocityComponent
                    {
                        velocity = direction * minionStats.speed
                    };
                }
            }).ScheduleParallel(Dependency);
        ecbs.AddJobHandleForProducer(finalJobHandle);

        Dependency = finalJobHandle;
    }
}
