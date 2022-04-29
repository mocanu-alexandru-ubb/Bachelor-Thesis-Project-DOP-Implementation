using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class GamePlayPhaseSystem : SystemBase
{
    public static bool isGamePlayPhase = false;
    EntityQuery getGamePlayPhaseComponents;
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
            .WithStructuralChanges()
            .WithStoreEntityQueryInField(ref getGamePlayPhaseComponents)
            .ForEach((ref GamePlayPhaseDataComponent gamePlayPhaseData) => {
                if (gamePlayPhaseData.phaseDuration > 0)
                {
                    gamePlayPhaseData.phaseDuration -= dt;
                    isGamePlayPhase = true;
                }
                else
                {
                    isGamePlayPhase = false;
                    EntityManager.DestroyEntity(getGamePlayPhaseComponents);
                }
        }).Run();
    }
}
