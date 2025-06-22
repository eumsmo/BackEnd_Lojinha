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


public partial struct PlayerAttackSystem : ISystem {
    private EntityManager entityManager;
    private Entity playerEntity;
    
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<PlayerData>();
    }

    public void OnUpdate(ref SystemState state) {
        entityManager = state.EntityManager;
        playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();

        if (entityManager.IsComponentEnabled<PlayerAttackCooldown>(playerEntity)) {
            var attackCooldown = entityManager.GetComponentData<PlayerAttackCooldown>(playerEntity);
            if (attackCooldown.timeLeft > 0f) {
                attackCooldown.timeLeft -= SystemAPI.Time.DeltaTime;
                entityManager.SetComponentData(playerEntity, attackCooldown);
                return;
            } else {
                entityManager.SetComponentEnabled<PlayerAttackCooldown>(playerEntity, false);
            }
        }

        PlayerAttackData playerAttackData = entityManager.GetComponentData<PlayerAttackData>(playerEntity);
        LocalTransform playerTransform = entityManager.GetComponentData<LocalTransform>(playerEntity);

        var spawnPosition = playerTransform.Position;
        var minDetectionSize = spawnPosition - playerAttackData.detectionRange;
        var maxDetectionSize = spawnPosition + playerAttackData.detectionRange;

        var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        var aabb = new OverlapAabbInput {
            Aabb = new Aabb {
                Min = minDetectionSize,
                Max = maxDetectionSize
            },
            Filter = playerAttackData.attackCollisionFilter,
        };

        var overlapResults = new NativeList<int>(state.WorldUpdateAllocator);
        if (!physicsWorldSingleton.PhysicsWorld.OverlapAabb(aabb, ref overlapResults)) return;

        var maxDist = float.MaxValue;
        var closestPosition = float3.zero;
        foreach (var entityIndex in overlapResults) {
            var body = physicsWorldSingleton.PhysicsWorld.Bodies[entityIndex];
            var entity = body.Entity;
            if (!entityManager.HasComponent<EnemyTag>(entity)) continue;

            var pos = body.WorldFromBody.pos;
            var dist = math.distancesq(spawnPosition.xy, pos.xy);
            if (dist < maxDist) {
                maxDist = dist;
                closestPosition = pos;
            }
        }

        var dirToEnemy = math.normalize(closestPosition.xy - spawnPosition.xy);
        var angleToEnemy = math.atan2(dirToEnemy.y, dirToEnemy.x);
        var spawnOrientation = quaternion.Euler(0f, 0f, angleToEnemy);


        var newAttack = entityManager.Instantiate(playerAttackData.attackPrefab);

        entityManager.SetComponentData(newAttack, LocalTransform.FromPositionRotation(spawnPosition, spawnOrientation));
        entityManager.AddComponentData(newAttack, new PlayerAttackTag());
        entityManager.AddComponentData(newAttack, new ProjectileData(playerAttackData.attackSpeed));

        entityManager.SetComponentEnabled<PlayerAttackCooldown>(playerEntity, true);
        var attackCooldownNovo = entityManager.GetComponentData<PlayerAttackCooldown>(playerEntity);
        attackCooldownNovo.timeLeft = playerAttackData.attackCooldown;
        entityManager.SetComponentData(playerEntity, attackCooldownNovo);

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