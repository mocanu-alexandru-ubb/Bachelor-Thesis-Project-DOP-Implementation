using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

[UpdateAfter(typeof(TileSelectSystem))]
public partial class HighlightTileSystem : SystemBase
{
    RenderMesh normalMesh;
    RenderMesh highlightMesh;

    protected override void OnStartRunning()
    {
        base.OnStartRunning();

        Entities
            .WithoutBurst()
            .ForEach((RenderMesh tileMesh, ref ClickableTileData clickableTileData) => {
                normalMesh = new RenderMesh
                {
                    mesh = tileMesh.mesh,
                    material = tileMesh.material,
                    subMesh = tileMesh.subMesh,
                    layer = tileMesh.layer,
                    castShadows = tileMesh.castShadows,
                    receiveShadows = tileMesh.receiveShadows,
                    needMotionVectorPass = tileMesh.needMotionVectorPass,
                    layerMask = tileMesh.layerMask
                };
                highlightMesh = new RenderMesh
                {
                    mesh = normalMesh.mesh,
                    material = MinionSpawnerPrefabConvertor.highlighMaterial,
                    subMesh = normalMesh.subMesh,
                    layer = normalMesh.layer,
                    castShadows = normalMesh.castShadows,
                    receiveShadows = normalMesh.receiveShadows,
                    needMotionVectorPass = normalMesh.needMotionVectorPass,
                    layerMask = normalMesh.layerMask
                };
                return;
            }).Run();
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
        Entities
            .WithoutBurst()
            .ForEach((Entity tile, RenderMesh tileMesh, ref ClickableTileData clickableTileData) => {
                if (clickableTileData.materialNeedsChange)
                {
                    if (clickableTileData.isHighlited)
                    {
                        ecb.SetSharedComponent(tile, highlightMesh);
                    }
                    else
                    {
                        var newMesh = new RenderMesh
                        {
                            mesh = normalMesh.mesh,
                            material = MinionSpawnerPrefabConvertor.normalMaterial,
                            subMesh = normalMesh.subMesh,
                            layer = normalMesh.layer,
                            castShadows = normalMesh.castShadows,
                            receiveShadows = normalMesh.receiveShadows,
                            needMotionVectorPass = normalMesh.needMotionVectorPass,
                            layerMask = normalMesh.layerMask
                        };
                        ecb.SetSharedComponent(tile, newMesh);
                    }
                    clickableTileData.materialNeedsChange = false;
                }
        }).Run();
        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
