using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;

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


public partial struct PlayerUISystem : ISystem {
    private EntityManager entityManager;
    private Entity playerEntity;
    
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerUIPrefab>();
    }

    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerUIPrefab>();

        var playerUI = entityManager.GetComponentData<PlayerUIPrefab>(playerEntity);

        if (!entityManager.HasComponent<PlayerUICanvas>(playerEntity)) {
            var newObj = Object.Instantiate(playerUI.prefab.Value);
            entityManager.AddComponent(playerEntity, typeof(PlayerUICanvas));
            entityManager.SetComponentData(playerEntity, new PlayerUICanvas {
                canvasTransform = newObj.GetComponent<Transform>(),
                healthSlider = newObj.GetComponentInChildren<Slider>()
            });

            return;
        }

        PlayerUICanvas playerUICanvas = entityManager.GetComponentData<PlayerUICanvas>(playerEntity);


        if (!entityManager.HasComponent<LocalToWorld>(playerEntity)) {
            if (playerUICanvas.canvasTransform.Value != null) {
                Object.Destroy(playerUICanvas.canvasTransform.Value.gameObject);
                entityManager.RemoveComponent<PlayerUICanvas>(playerEntity);
            }
            return;
        }

        LocalToWorld playerTransform = entityManager.GetComponentData<LocalToWorld>(playerEntity);
        HealthData healthData = entityManager.GetComponentData<HealthData>(playerEntity);

        playerUICanvas.canvasTransform.Value.position = playerTransform.Position;
        playerUICanvas.healthSlider.Value.value = (float) healthData.health / healthData.maxHealth;
    }
}