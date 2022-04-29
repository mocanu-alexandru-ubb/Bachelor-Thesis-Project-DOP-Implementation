using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class VelocitySystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        bool isGamePlayPhase = GamePlayPhaseSystem.isGamePlayPhase;
        if (!isGamePlayPhase) return;

        Entities
            .ForEach((ref Translation translation, in VelocityComponent velocity, in MinionComponent minion) =>
            {
                translation.Value += velocity.velocity * deltaTime * minion.speed;
            }).ScheduleParallel();
    }
}
