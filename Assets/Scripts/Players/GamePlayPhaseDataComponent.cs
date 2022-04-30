using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct GamePlayPhaseDataComponent : IComponentData
{
    public float phaseDuration;
    public float phaseTimer;
}
