using UnityEngine;
using Unity.Entities;
using Unity.Burst;

public struct AtualizarXPFromMonoToDOTS: IComponentData, IEnableableComponent {
    public uint xp;
}

[BurstCompile]
public partial struct AtualizarXPFromMonoToDOTSSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<AtualizarXPFromMonoToDOTS>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var entity = SystemAPI.GetSingletonEntity<PlayerData>();
        if (!SystemAPI.IsComponentEnabled<AtualizarXPFromMonoToDOTS>(entity)) return;
        var val = SystemAPI.GetComponent<AtualizarXPFromMonoToDOTS>(entity);

        var player = SystemAPI.GetComponent<PlayerData>(entity);
        player.xp = val.xp;
        SystemAPI.SetComponent(entity, player);
        SystemAPI.SetComponentEnabled<AtualizarXPFromMonoToDOTS>(entity, false);
    }
}