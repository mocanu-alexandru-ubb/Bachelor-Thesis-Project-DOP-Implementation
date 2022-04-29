using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MinionSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        bool isGamePlayPhase = GamePlayPhaseSystem.isGamePlayPhase;
        if (!isGamePlayPhase) return;
        float dt = Time.DeltaTime;

        Entities
            .WithName("Spawning_Entities")
            .WithStructuralChanges()
            .ForEach((ref SpawnerComponentData spawnerComponent) =>
            {
                if (spawnerComponent.spawnCooldown <= 0)
                {
                    Entity minion = EntityManager.Instantiate(MinionSpawnerPrefabConvertor.minionToSpawn);
                    EntityManager.SetComponentData(minion, new Translation { Value = spawnerComponent.spawnPosition });
                    EntityManager.AddComponentData(minion, spawnerComponent.targetNode);
                    spawnerComponent.spawnCooldown = 1 / spawnerComponent.spawnsPerSecond;
                }
                else
                {
                    spawnerComponent.spawnCooldown -= dt;
                }
            }).Run();
    }
}
