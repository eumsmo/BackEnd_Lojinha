using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics.Systems;

public struct PlayerAttackData : IComponentData {
    public Entity attackPrefab;
    public float attackCooldown;
    public float attackSpeed;
    public float projectileDuration;
    public float3 detectionRange;
    public CollisionFilter attackCollisionFilter;
}

public struct PlayerAttackCooldown : IComponentData, IEnableableComponent {
    public float timeLeft;

    public PlayerAttackCooldown(float timeLeft) {
        this.timeLeft = timeLeft;
    }
}

public struct PlayerAttackTag : IComponentData {}


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
        entityManager.AddComponentData(newAttack, new AutoDestruct(playerAttackData.projectileDuration));

        entityManager.SetComponentEnabled<PlayerAttackCooldown>(playerEntity, true);
        var attackCooldownNovo = entityManager.GetComponentData<PlayerAttackCooldown>(playerEntity);
        attackCooldownNovo.timeLeft = playerAttackData.attackCooldown;
        entityManager.SetComponentData(playerEntity, attackCooldownNovo);

    }
}


[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct TriggerAttackSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var attackJob = new PlayerAttackJob {
            DeltaTime = SystemAPI.Time.DeltaTime,
            playerAttackDataLookup = SystemAPI.GetComponentLookup<PlayerAttackTag>(true),
            attackDataLookup = SystemAPI.GetComponentLookup<AttackData>(true),
            enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            cooldownLookup = SystemAPI.GetComponentLookup<AttackInCooldown>(),
            damageBufferLookup = SystemAPI.GetBufferLookup<DamageThisFrame>()
        };

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = attackJob.Schedule(simulation, state.Dependency);
    }

    public struct PlayerAttackJob : ITriggerEventsJob {
        public float DeltaTime;
        [ReadOnly] public ComponentLookup<PlayerAttackTag> playerAttackDataLookup;
        [ReadOnly] public ComponentLookup<AttackData> attackDataLookup;
        [ReadOnly] public ComponentLookup<EnemyTag> enemyLookup;
        public ComponentLookup<AttackInCooldown> cooldownLookup;
        public BufferLookup<DamageThisFrame> damageBufferLookup;

        public void Execute(TriggerEvent triggerEvent) {
            Entity attackEntity;
            Entity enemyEntity;

            if (enemyLookup.HasComponent(triggerEvent.EntityA) && playerAttackDataLookup.HasComponent(triggerEvent.EntityB)) {
                attackEntity = triggerEvent.EntityB;
                enemyEntity = triggerEvent.EntityA;
            } else if (enemyLookup.HasComponent(triggerEvent.EntityB) && playerAttackDataLookup.HasComponent(triggerEvent.EntityA)) {
                attackEntity = triggerEvent.EntityA;
                enemyEntity = triggerEvent.EntityB;
            } else {
                return; // Not a valid attack-enemy collision
            }

            if (cooldownLookup.IsComponentEnabled(enemyEntity)) return;
            var attackData = attackDataLookup[attackEntity];
            cooldownLookup[enemyEntity] = new AttackInCooldown(attackData.cooldown);
            cooldownLookup.SetComponentEnabled(enemyEntity, true);

            var playerDamageBuffer = damageBufferLookup[enemyEntity];
            playerDamageBuffer.Add(new DamageThisFrame { damage = attackData.damage });
        }
    }
}


