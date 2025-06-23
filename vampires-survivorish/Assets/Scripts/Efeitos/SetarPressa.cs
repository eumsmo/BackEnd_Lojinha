using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

public struct SetarPressa: IComponentData, IEnableableComponent {
    public float mult;
}

[BurstCompile]
public partial struct SetarPressaSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SetarPressa>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<SetarPressa>(entity)) return;
        var efeito = SystemAPI.GetComponent<SetarPressa>(entity);

        var movimento = SystemAPI.GetComponent<MovementData>(entity);
        movimento.speed *= 1f + efeito.mult;
        SystemAPI.SetComponent(entity, movimento);


        SystemAPI.SetComponentEnabled<SetarPressa>(entity, false);
    }
}