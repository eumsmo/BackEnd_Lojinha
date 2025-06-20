using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct EnemyMovementSystem : ISystem {
    private EntityManager entityManager;
    private Entity playerEntity;
    
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<MovementData>();
    }
    
    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        
        foreach (var (enemyTransform, enemyMovement) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovementData>>().WithNone<PlayerData>()) {
            float3 playerPosition = playerTransform.Position;
            float3 enemyPosition = enemyTransform.ValueRW.Position;
            float2 directionToPlayer = math.normalize(playerPosition.xy - enemyPosition.xy);

            enemyTransform.ValueRW.Position += new float3(directionToPlayer * enemyMovement.ValueRO.speed * SystemAPI.Time.DeltaTime, 0f);
        }
    }

}
