using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class StandardTurretFireSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!GamePlayPhaseSystem.isGamePlayPhase) return;
        float deltaTime = Time.DeltaTime;

        Entities
            .WithName("Standart_Turret_Fire")
            .WithStructuralChanges()
            .ForEach((ref FireRateComponent fireRate, in StandardTurretFireComponent fireComponent, in MinionSeekingComponent target, in BulletSpawningPoint bulletSpawningPoint) => {
                if (fireRate.cooldown <= 0 && !target.targetPosition.Equals(float3.zero))
                {
                    Entity bullet = EntityManager.Instantiate(MinionSpawnerPrefabConvertor.bulletToSpawn);
                    EntityManager.SetComponentData(bullet,
                        new Translation { Value = new float3(bulletSpawningPoint.spawnPosition) });
                    EntityManager.SetComponentData(bullet,
                        new BasicBulletFollowComponent { targetId = target.targetId });
                    fireRate.cooldown = 1f / fireRate.rateOfFire;
                }
                else
                {
                    fireRate.cooldown -= deltaTime;
                }
        }).Run();
    }
}
