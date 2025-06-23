using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

public struct SetarMouse: IComponentData, IEnableableComponent {
    public float multCooldown;
    public float multSpeed;
}

[BurstCompile]
public partial struct SetarMouseSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SetarMouse>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<SetarMouse>(entity)) return;
        var efeito = SystemAPI.GetComponent<SetarMouse>(entity);

        var attack = SystemAPI.GetComponent<PlayerAttackData>(entity);
        attack.attackCooldown *=  1 + efeito.multCooldown;
        attack.attackSpeed *= 1 + efeito.multSpeed;
        SystemAPI.SetComponent(entity, attack);

        SystemAPI.SetComponentEnabled<SetarMouse>(entity, false);
    }
}