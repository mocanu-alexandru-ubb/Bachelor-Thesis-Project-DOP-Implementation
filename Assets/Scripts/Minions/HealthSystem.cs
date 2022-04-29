using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class HealthSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithStructuralChanges()
            .ForEach((Entity minionEntity, ref MinionComponent minionComponent, in TakeDamageComponent damageComponent) => {
                minionComponent.currentHealth -= damageComponent.damageToTake;
                if (minionComponent.currentHealth < 0)
                {
                    EntityManager.DestroyEntity(minionEntity);
                }
                else
                {
                    EntityManager.RemoveComponent(minionEntity, typeof(TakeDamageComponent));
                }
        }).Run();
    }
}
