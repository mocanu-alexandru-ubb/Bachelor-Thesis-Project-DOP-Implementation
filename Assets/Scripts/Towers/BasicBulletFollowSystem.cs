using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class BasicBulletFollowSystem : SystemBase
{
    private float speed = 5f;
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

        Entities
            .WithStoreEntityQueryInField(ref dataCollectionQuery)
            .ForEach((Entity minionEntity, in MinionComponent minionTag, in LocalToWorld transform, in EnitityUniqueIdComponent enitityUniqueId) =>
            {
                mapWrriter.TryAdd(enitityUniqueId.id, new MinionData
                {
                    position = transform.Position,
                    entity = minionEntity
                });
            }).Schedule();

        Entities
            .WithStructuralChanges()
            .WithReadOnly(minionMap)
            .WithDisposeOnCompletion(minionMap)
            .ForEach((Entity bullet, ref Translation transform, ref BasicBulletFollowComponent followTarget) =>
            {
                if (!minionMap.ContainsKey(followTarget.targetId))
                {
                    EntityManager.DestroyEntity(bullet);
                }
                else
                {
                    float3 targetPosition = minionMap[followTarget.targetId].position;
                    Vector3 direction = targetPosition - transform.Value;
                    float distanceToMove = speed * deltaTime;
                    if (direction.sqrMagnitude < distanceToMove * distanceToMove)
                    {
                        Entity bulletDestroy = EntityManager.Instantiate(MinionSpawnerPrefabConvertor.bulletDesctructionToSpawn);
                        EntityManager.SetComponentData(bulletDestroy,
                            new Translation { Value = new float3(transform.Value) });
                        EntityManager.AddComponentData(minionMap[followTarget.targetId].entity, new TakeDamageComponent { damageToTake = 1 });
                    }
                    transform.Value += new float3(direction.normalized * distanceToMove);
                }
            }).Run();
    }
}
