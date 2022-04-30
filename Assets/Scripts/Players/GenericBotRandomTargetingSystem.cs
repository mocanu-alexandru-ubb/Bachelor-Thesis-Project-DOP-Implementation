using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial class GenericBotRandomTargetingSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var lookup = GetBufferFromEntity<ChooseEnemyRandomlyList>();
        Entities
            .WithReadOnly(lookup)
            .ForEach((Entity self, ref SpawnerComponentData spawnerComponentData, in GenericBotTag botTag) => {
                var buffer = lookup[self];
                spawnerComponentData.targetNode = buffer[Random.Range(0, buffer.Length)].Value;
            }).Run();
    }
}
