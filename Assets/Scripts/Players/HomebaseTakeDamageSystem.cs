using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class HomebaseTakeDamageSystem : SystemBase
{
    private EntityQuery query;

    private struct HomebaseTakeDamageDTO
    {
        public Entity homebse;
        public HomebaseHealthData health;
    }

    protected override void OnUpdate()
    {
        int entitiesInQuery = query.CalculateEntityCount();
        NativeHashMap<int, HomebaseTakeDamageDTO> homebases = new NativeHashMap<int, HomebaseTakeDamageDTO>(entitiesInQuery, Allocator.TempJob);
        var mapWriter = homebases.AsParallelWriter();

        Entities
            .WithStoreEntityQueryInField(ref query)
            .ForEach((Entity self, ref HomebaseHealthData homebaseHealth, in PathfindingNodeData nodeData) =>
            {
                if (nodeData.isHomebase)
                    mapWriter.TryAdd(nodeData.nodeId, new HomebaseTakeDamageDTO { homebse = self, health = homebaseHealth});
            }).ScheduleParallel();

        Entities
            .WithStructuralChanges()
            .WithDisposeOnCompletion(homebases)
            .ForEach((Entity self, in HomebaseTakeDamageInstance homebaseTakeDamage) => {
                if (homebases.ContainsKey(homebaseTakeDamage.nodeId))
                {
                    var dto = homebases[homebaseTakeDamage.nodeId];
                    var newHealthComponent = new HomebaseHealthData { health = dto.health.health - homebaseTakeDamage.damageToTake };
                    if (newHealthComponent.health <= 0)
                    {

                    }
                    dto.health = newHealthComponent;
                    EntityManager.SetComponentData(dto.homebse, newHealthComponent);
                }
                EntityManager.DestroyEntity(self);
        }).Run();
    }
}
