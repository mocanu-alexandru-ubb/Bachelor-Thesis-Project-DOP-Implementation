using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public partial class RotateTowerToTargetSystem : SystemBase
{
    private const float damping = 20f;
    protected override void OnUpdate()
    {
        if (!GamePlayPhaseSystem.isGamePlayPhase) return;
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in LocalToWorld translation, in MinionSeekingComponent target) =>
        {
            if (target.targetId != -1)
            {
                var lookPos = target.targetPosition - translation.Position;
                lookPos.y = 0;
                var finalRotation = Quaternion.LookRotation(lookPos);
                rotation.Value = Quaternion.Slerp(rotation.Value, finalRotation, deltaTime * damping);

            }
        }).ScheduleParallel();
    }
}
