using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class TurnOrderSystem : SystemBase
{
    NativeQueue<Entity> playerQueue = new NativeQueue<Entity>(Allocator.Persistent);

    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        Entities
            .WithoutBurst()
            .WithAny<GenericBotTag>()
            .ForEach((Entity self) => {
                playerQueue.Enqueue(self);
        }).Run();
        Entity gameplayPhase = EntityManager.CreateEntity();
        playerQueue.Enqueue(gameplayPhase);
        EntityManager.AddComponentData(gameplayPhase, new GamePlayPhaseDataComponent
        {
            phaseDuration = 6,
            phaseTimer = 6
        });
        EntityManager.SetName(gameplayPhase, "gamePlayPhase");
    }

    protected override void OnUpdate()
    {
        EntityQuery someoneHasTurn = GetEntityQuery(typeof(HasTurnTag));
        if (someoneHasTurn.CalculateEntityCount() == 0)
        {
            Entity nextPlayer = playerQueue.Dequeue();
            EntityManager.AddComponent<HasTurnTag>(nextPlayer);
            playerQueue.Enqueue(nextPlayer);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        playerQueue.Dispose();
    }
}
