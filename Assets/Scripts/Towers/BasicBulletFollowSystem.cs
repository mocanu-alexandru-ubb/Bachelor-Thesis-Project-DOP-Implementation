using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BasicBulletFollowSystem : SystemBase
{
    public float speed = 5f;
    private EntityQuery dataCollectionQuery;

    private struct MinionData
    {
        public float3 position;
        public Entity entity;
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        NativeHashMap<int, MinionData> minionMap =  new NativeHashMap<int, MinionData>(dataCollectionQuery.CalculateEntityCount(), Allocator.TempJob);
        var mapWrriter = minionMap.AsParallelWriter();

        var dataCollectionJob = Entities
            .WithStoreEntityQueryInField(ref dataCollectionQuery)
            .ForEach((Entity minionEntity, in MinionComponent minionTag, in LocalToWorld transform, in EnitityUniqueIdComponent enitityUniqueId) =>
            {
                mapWrriter.TryAdd(enitityUniqueId.id, new MinionData
                {
                    position = transform.Position,
                    entity = minionEntity
                });
            }).ScheduleParallel(Dependency);

        EntityCommandBufferSystem ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer.ParallelWriter parallelEcb = ecbs.CreateCommandBuffer().AsParallelWriter();

        Entity bulletDestruction = MinionSpawnerPrefabConvertor.bulletDesctructionToSpawn;
        float localSpeed = speed;

        var jobHandle = Entities
            .WithReadOnly(minionMap)
            .WithDisposeOnCompletion(minionMap)
            .WithDisposeOnCompletion(parallelEcb)
            .ForEach((int entityInQueryIndex, Entity bullet, ref Translation transform, ref BasicBulletFollowComponent followTarget) =>
            {
                if (!minionMap.ContainsKey(followTarget.targetId))
                {
                    parallelEcb.DestroyEntity(entityInQueryIndex, bullet);
                }
                else
                {
                    float3 targetPosition = minionMap[followTarget.targetId].position;
                    Vector3 direction = targetPosition - transform.Value;
                    float distanceToMove = localSpeed * deltaTime;
                    if (direction.sqrMagnitude < distanceToMove * distanceToMove)
                    {
                        Entity bulletDestroy = parallelEcb.Instantiate(entityInQueryIndex, bulletDestruction);
                        parallelEcb.SetComponent(entityInQueryIndex, bulletDestroy, new Translation { Value = new float3(transform.Value) });
                        parallelEcb.AddComponent(entityInQueryIndex, minionMap[followTarget.targetId].entity, new TakeDamageComponent { damageToTake = 1 });
                    }
                    transform.Value += new float3(direction.normalized * distanceToMove);
                }
            }).ScheduleParallel(dataCollectionJob);
        ecbs.AddJobHandleForProducer(jobHandle);
        Dependency = jobHandle;
    }
}
