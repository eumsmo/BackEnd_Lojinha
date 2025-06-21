using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;

public partial struct PlayerSystem : ISystem {
    private EntityManager entityManager;
    private Entity playerEntity;
    private MovementData movementData;
    
    private Entity inputEntity;
    private InputData inputData;
    
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerData>();
    }

    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        movementData = entityManager.GetComponentData<MovementData>(playerEntity);

        inputEntity = SystemAPI.GetSingletonEntity<InputData>();
        inputData = entityManager.GetComponentData<InputData>(inputEntity);

        movementData.direction = inputData.moveDirection;
        state.EntityManager.SetComponentData(playerEntity, movementData);
    }
}
