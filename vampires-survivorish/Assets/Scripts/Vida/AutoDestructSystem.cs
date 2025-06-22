using System;
using Unity.Entities;
using Unity.Collections;

public struct AutoDestruct : IComponentData, IEnableableComponent {
    public float timeLeft;

    public AutoDestruct(float timeLeft) {
        this.timeLeft = timeLeft;
    }
}

public partial struct AutoDestructSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        var deltaTime = SystemAPI.Time.DeltaTime;

        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        foreach (var (autoDestruct, entity) in SystemAPI.Query<RefRW<AutoDestruct>>().WithEntityAccess()) {
            if (autoDestruct.ValueRO.timeLeft > 0f) {
                autoDestruct.ValueRW.timeLeft -= deltaTime;
                if (autoDestruct.ValueRO.timeLeft > 0f) {
                    continue;
                }
            }

            ecb.SetComponentEnabled<AutoDestruct>(entity, false);

            if (!state.EntityManager.HasComponent<DestroyableTag>(entity)) {
                ecb.AddComponent<DestroyableTag>(entity);
            }

            ecb.SetComponentEnabled<DestroyableTag>(entity, true);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}