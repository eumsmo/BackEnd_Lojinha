using Unity.Entities;
using Unity.Physics;

public struct CollisionSetup : IComponentData, IEnableableComponent {
    public CollisionFilter collisionFilter;
}

public partial struct CollisionSetupSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<CollisionSetup>();
    }

    public void OnUpdate(ref SystemState state) {
        foreach (var (col, setup, setupEnabled) in SystemAPI.Query<RefRW<PhysicsCollider>, RefRO<CollisionSetup>, EnabledRefRW<CollisionSetup>>()) {
            CollisionFilter filter = setup.ValueRO.collisionFilter;
            col.ValueRW.Value.Value.SetCollisionFilter(filter);
            setupEnabled.ValueRW = false;
        }
    }
}