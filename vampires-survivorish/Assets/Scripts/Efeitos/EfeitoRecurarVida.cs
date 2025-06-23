using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Burst;

public struct EfeitoRecurarVida: IComponentData, IEnableableComponent {
    public uint vidaRecuperada;
}

[BurstCompile]
public partial struct EfeitoRecurarVidaSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EfeitoRecurarVida>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<EfeitoRecurarVida>(entity)) return;
        var efeito = SystemAPI.GetComponent<EfeitoRecurarVida>(entity);

        var vida = SystemAPI.GetComponent<HealthData>(entity);
        vida.health = math.min(vida.health + efeito.vidaRecuperada, vida.maxHealth);
        SystemAPI.SetComponent(entity, vida);
        SystemAPI.SetComponentEnabled<EfeitoRecurarVida>(entity, false);
    }
}