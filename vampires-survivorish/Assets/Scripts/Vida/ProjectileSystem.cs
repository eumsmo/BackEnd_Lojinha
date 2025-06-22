using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Transforms;

public struct ProjectileData : IComponentData {
    public float speed;

    public ProjectileData(float speed) {
        this.speed = speed;
    }
}

public partial struct ProjectileSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (projectile, transform) in SystemAPI.Query<RefRO<ProjectileData>, RefRW<LocalTransform>>()) {
            transform.ValueRW.Position += transform.ValueRW.Right() * projectile.ValueRO.speed * deltaTime;
        }
    }
}

