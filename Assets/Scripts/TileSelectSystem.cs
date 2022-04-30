using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class TileSelectSystem : SystemBase
{
    public LayerMask clickableLayer = LayerMask.GetMask("ClickableLayer");
    private RaycastHit raycastHit;
    protected override void OnUpdate()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out raycastHit, 18, clickableLayer))
        {
            float3 hitPosition = raycastHit.point;

            Entities
                .ForEach((ref ClickableTileData clickableTile, in LocalToWorld transform, in NonUniformScale scale) =>
                {
                    float3 tilePosition = transform.Position;
                    tilePosition.x -= scale.Value.x / 2;
                    tilePosition.z -= scale.Value.z / 2;
                    if (tilePosition.x <= hitPosition.x && hitPosition.x <= tilePosition.x + scale.Value.x
                    && tilePosition.z <= hitPosition.z && hitPosition.z <= tilePosition.z + scale.Value.z)
                    {
                        if (!clickableTile.isHighlited)
                        {
                            clickableTile.isHighlited = true;
                            clickableTile.materialNeedsChange = true;
                        }
                    }
                    else
                    {
                        if (clickableTile.isHighlited)
                        {
                            clickableTile.isHighlited = false;
                            clickableTile.materialNeedsChange = true;
                        }
                    }
                })
                .ScheduleParallel();
        }
        else
        {
        }
    }
}
