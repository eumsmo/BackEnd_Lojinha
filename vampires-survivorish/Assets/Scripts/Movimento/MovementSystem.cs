using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.Entities.UniversalDelegates;

public struct MovementInitializeTag : IComponentData, IEnableableComponent { }


public partial struct MovementInitializer : ISystem {

    public void OnUpdate(ref SystemState state) {
        foreach (var (physicsMass, tag) in SystemAPI.Query<RefRW<PhysicsMass>, EnabledRefRW<MovementInitializeTag>>()) {
            physicsMass.ValueRW.InverseInertia = float3.zero;
            tag.ValueRW = false; // Disable the tag after initialization
        }
    }
}

public partial struct MovementSystem : ISystem {
    private EntityManager entityManager;

    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MovementData>();
    }
    
    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        foreach (var (velocity, movementData, entity) in SystemAPI.Query<RefRW<PhysicsVelocity>, RefRO<MovementData>>().WithEntityAccess()) {
            float directionX = math.sign(movementData.ValueRO.direction.x);
            if (directionX != 0f) {
                Entity childEntity = entityManager.GetBuffer<Child>(entity).Length > 0 ? entityManager.GetBuffer<Child>(entity)[0].Value : Entity.Null;
                if (childEntity != Entity.Null) {
                    var facing = entityManager.GetComponentData<FacingDirectionData>(childEntity);
                    facing.direction = directionX;
                    entityManager.SetComponentData(childEntity, facing);
                }
            }
            
            velocity.ValueRW.Linear = new float3(movementData.ValueRO.direction * movementData.ValueRO.speed * 10f * SystemAPI.Time.DeltaTime, 0f);
        }
    }
}
