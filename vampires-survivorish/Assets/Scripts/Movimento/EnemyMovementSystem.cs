using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;


public partial struct EnemyMovementSystem : ISystem {
    
    private EntityManager entityManager;
    private Entity playerEntity;
    
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MovementData>();
    }
    
    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;

        if (!SystemAPI.HasSingleton<PlayerData>()) {
            return; // Exit if there is no player entity
        }
        
        playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        
        foreach (var (enemyTransform, enemyMovement) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MovementData>>().WithNone<PlayerData>()) {
            float3 playerPosition = playerTransform.Position;
            float3 enemyPosition = enemyTransform.ValueRO.Position;
            float2 directionToPlayer = math.normalize(playerPosition.xy - enemyPosition.xy);

            enemyMovement.ValueRW.direction = directionToPlayer;
        }
    }

}
