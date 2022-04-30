using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public partial class GenericBotSystem : SystemBase
{
    protected override void OnUpdate()
    {
        EntityCommandBufferSystem ecbs = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
        EntityCommandBuffer.ParallelWriter parallelEcb = ecbs.CreateCommandBuffer().AsParallelWriter();
        var jobHandle = Entities
            .WithDisposeOnCompletion(parallelEcb)
            .WithoutBurst()
            .ForEach((int entityInQueryIndex, Entity self, ref MoneyComponent moneyComponent, in GenericBotTag botTag, in HasTurnTag hasTurnTag) => {
                if (moneyComponent.money >= 100)
                {
                    parallelEcb.AppendToBuffer(entityInQueryIndex, self, new MinionSpawnQueue { Value = MinionSpawnerPrefabConvertor.minionToSpawn });
                    moneyComponent.money -= 100;
                }
                else
                {
                    parallelEcb.RemoveComponent<HasTurnTag>(entityInQueryIndex, self);
                }
        }).Schedule(Dependency);

        ecbs.AddJobHandleForProducer(jobHandle);
        Dependency = jobHandle;
    }
}
