using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial struct PlayerSystem : ISystem {
    private EntityManager entityManager;
    private Entity playerEntity;
    private MovementData movementData;
    
    private Entity inputEntity;
    private InputData inputData;
    
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerData>();
    }

    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        movementData = entityManager.GetComponentData<MovementData>(playerEntity);

        inputEntity = SystemAPI.GetSingletonEntity<InputData>();
        inputData = entityManager.GetComponentData<InputData>(inputEntity);

        float2 moveVector = inputData.moveDirection;

        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);
        playerTransform.Position += new float3(moveVector * movementData.speed * SystemAPI.Time.DeltaTime, 0f);
        state.EntityManager.SetComponentData(playerEntity, playerTransform);
    }
}
