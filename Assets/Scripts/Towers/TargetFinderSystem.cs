using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(RotateTowerToTargetSystem)), UpdateBefore(typeof(StandardTurretFireSystem))]
[BurstCompile]
public partial class TargetFinderSystem : SystemBase
{
    private EntityQuery query;

    private struct MinionTargetingData
    {
        public float3 position;
        public int id;
        public int owner;
    }

    protected override void OnUpdate()
    {
        if (!GamePlayPhaseSystem.isGamePlayPhase) return;

        int entitiesInQuery = query.CalculateEntityCount();
        NativeList<MinionTargetingData> possibleTargets = new NativeList<MinionTargetingData>(entitiesInQuery, Allocator.TempJob);
        JobHandle dataCollection = Entities
            .WithName("Load_Minions_As_Array")
            .WithStoreEntityQueryInField(ref query)
            .WithAll<MinionComponent>()
            .ForEach((in LocalToWorld translation, in EnitityUniqueIdComponent enitityUniqueId, in MinionComponent minionComponent) =>
            {
                possibleTargets.Add(new MinionTargetingData
                {
                    position = translation.Position,
                    id = enitityUniqueId.id,
                    owner = minionComponent.owner
                });
            }).Schedule(Dependency);

        JobHandle final = Entities
            .WithReadOnly(possibleTargets)
            .WithName("Find_Best_Target_For_Towers")
            .WithDisposeOnCompletion(possibleTargets)
            .ForEach((ref MinionSeekingComponent minionSeeking, in TargetFinderComponent targetFinder, in LocalToWorld translation, in TowerStats towerStats) => {
                float minDistance = float.MaxValue;
                float3 bestTarget = float3.zero;
                int bestTargetId = -1;
                for (int i = 0; i < possibleTargets.Length; i++)
                {
                    if (possibleTargets[i].owner != towerStats.owner)
                    {
                        float currentDistance = math.distancesq(translation.Position, possibleTargets[i].position);

                        if (currentDistance < targetFinder.towerRange && currentDistance < minDistance)
                        {
                            minDistance = currentDistance;
                            bestTarget = possibleTargets[i].position;
                            bestTargetId = possibleTargets[i].id;
                        }
                    }
                }
                minionSeeking = new MinionSeekingComponent { targetPosition = bestTarget, targetId = bestTargetId };
            }).ScheduleParallel(dataCollection);
        Dependency = final;
    }
}
