using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class MinionSpawnerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        if (!GamePlayPhaseSystem.isGamePlayPhase) return;
        float dt = Time.DeltaTime;
        var lookup = GetBufferFromEntity<MinionSpawnQueue>();

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithName("Spawning_Entities")
            .ForEach((int entityInQueryIndex, Entity self, ref SpawnerComponentData spawnerComponent, in LocalToWorld transform, in PathfindingNodeData homebaseNode) =>
            {
                if (spawnerComponent.spawnCooldown <= 0)
                {
                    var buffer = lookup[self];
                    if (!buffer.IsEmpty)
                    {
                        Entity minion = ecb.Instantiate(buffer.ElementAt(0).Value);
                        buffer.RemoveAt(0);
                        ecb.SetComponent(minion, new Translation { Value = spawnerComponent.spawnPosition });
                        ecb.AddComponent(minion, spawnerComponent.targetNode);
                        ecb.SetComponent(minion, new MinionComponent
                        {
                            maxHealth = 1,
                            currentHealth = 1,
                            owner = homebaseNode.nodeId,
                            damage = 1,
                            speed = 1

                        });

                        float3 direction = float3.zero;
                        float diffX = spawnerComponent.targetNode.nodePosition.x - transform.Position.x;
                        float diffZ = spawnerComponent.targetNode.nodePosition.z - transform.Position.z;
                        direction.x = math.abs(diffX) < 0.2 ? 0 : diffX / math.abs(diffX);
                        direction.z = math.abs(diffZ) < 0.2 ? 0 : diffZ / math.abs(diffZ);


                        ecb.SetComponent(minion, new VelocityComponent
                        {
                            velocity = direction * 1
                        });

                        spawnerComponent.spawnCooldown = 1 / spawnerComponent.spawnsPerSecond;
                    }
                }
                else
                {
                    spawnerComponent.spawnCooldown -= dt;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
