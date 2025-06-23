using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

public struct SetarIntrospecto: IComponentData, IEnableableComponent {
    public int defesa;
}

[BurstCompile]
public partial struct SetarIntrospectoSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SetarIntrospecto>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<SetarIntrospecto>(entity)) return;
        var efeito = SystemAPI.GetComponent<SetarIntrospecto>(entity);

        var health = SystemAPI.GetComponent<HealthData>(entity);
        health.defense += (uint) efeito.defesa;
        SystemAPI.SetComponent(entity, health);

        SystemAPI.SetComponentEnabled<SetarIntrospecto>(entity, false);
    }
}