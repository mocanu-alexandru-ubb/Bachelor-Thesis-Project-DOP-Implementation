using Unity.Entities;
using Unity.Jobs;

public partial class EnityUniqueIdSystem : SystemBase
{
    private int lastFreeId = 1000;
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .ForEach((ref EnitityUniqueIdComponent idComponent) => {
                if (idComponent.id == 0)
                {
                    idComponent.id = lastFreeId++; 
                }
        }).Run();
    }
}
