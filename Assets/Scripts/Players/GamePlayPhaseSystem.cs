using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class GamePlayPhaseSystem : SystemBase
{
    public static bool isGamePlayPhase = false;
    private bool giveOutMoney = false;
    protected override void OnUpdate()
    {
        float dt = Time.DeltaTime;
        Entities
            .WithStructuralChanges()
            .ForEach((Entity self, ref GamePlayPhaseDataComponent gamePlayPhaseData, in HasTurnTag hasTurn) => {
                if (gamePlayPhaseData.phaseTimer > 0)
                {
                    gamePlayPhaseData.phaseTimer -= dt;
                    isGamePlayPhase = true;
                }
                else
                {
                    gamePlayPhaseData.phaseTimer = gamePlayPhaseData.phaseDuration;
                    isGamePlayPhase = false;
                    EntityManager.RemoveComponent<HasTurnTag>(self);
                    giveOutMoney = true;
                }
        }).Run();

        if (giveOutMoney)
        {
            Entities
                .ForEach((ref MoneyComponent moneyComponent) => {
                    moneyComponent.money += 300;
                }).ScheduleParallel();
            giveOutMoney = false;
        }
    }
}
