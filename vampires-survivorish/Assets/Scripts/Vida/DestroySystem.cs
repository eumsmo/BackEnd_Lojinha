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

        foreach (var (destroyable, entity) in SystemAPI.Query<DestroyableTag>().WithEntityAccess()) {
            var children = SystemAPI.GetBuffer<Child>(entity);
            if (children.Length > 0) {
                foreach (var child in children) {
                    ecb.DestroyEntity(child.Value);
                }
            }

            ecb.DestroyEntity(entity);
        }
    }
}
