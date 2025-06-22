using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public struct DestroyableTag : IComponentData, IEnableableComponent { }

[UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
[UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
public partial struct DestroySystem : ISystem {

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }
    
    public void OnUpdate(ref SystemState state) {
        var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        var beginEcbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var beginEcb = beginEcbSystem.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (destroyable, entity) in SystemAPI.Query<DestroyableTag>().WithEntityAccess()) {
            var children = SystemAPI.GetBuffer<Child>(entity);
            if (children.Length > 0) {
                foreach (var child in children) {
                    ecb.DestroyEntity(child.Value);
                }
            }

            if (SystemAPI.HasComponent<DropData>(entity)) {
                DropData dropData = SystemAPI.GetComponent<DropData>(entity);
                if (dropData.dropPrefab != Entity.Null) {
                    var dropEntity = beginEcb.Instantiate(dropData.dropPrefab);
                    beginEcb.SetComponent(dropEntity, LocalTransform.FromPosition(SystemAPI.GetComponent<LocalTransform>(entity).Position));
                }
            }

            ecb.DestroyEntity(entity);
        }
    }
}
