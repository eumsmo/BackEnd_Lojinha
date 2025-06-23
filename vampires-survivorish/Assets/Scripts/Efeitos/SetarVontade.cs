using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

public struct SetarVontade: IComponentData, IEnableableComponent {
    public int extra;
}

[BurstCompile]
public partial struct SetarVontadeSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SetarVontade>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<SetarVontade>(entity)) return;
        var efeito = SystemAPI.GetComponent<SetarVontade>(entity);

        var attack = SystemAPI.GetComponent<PlayerAttackData>(entity);
        attack.damage += (uint) efeito.extra;
        SystemAPI.SetComponent(entity, attack);

        SystemAPI.SetComponentEnabled<SetarVontade>(entity, false);
    }
}