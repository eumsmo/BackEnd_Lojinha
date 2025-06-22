using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Transforms;

public class AttackAuthoring : MonoBehaviour {
    public uint damage = 3;
    public float cooldown = 0.5f;

    class Baker : Baker<AttackAuthoring> {
        public override void Bake(AttackAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AttackData attackData = new AttackData(authoring.damage, authoring.cooldown);
            AddComponent(entity, attackData);
            AddComponent<AttackInCooldown>(entity);
            SetComponentEnabled<AttackInCooldown>(entity, false);
        }
    }
}

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



public struct AttackData : IComponentData {
    public uint damage;
    public float cooldown;

    public AttackData(uint damage, float cooldown) {
        this.damage = damage;
        this.cooldown = cooldown;
    }
}

public struct AttackInCooldown : IComponentData, IEnableableComponent {
    public float timeLeft;

    public AttackInCooldown(float timeLeft) {
        this.timeLeft = timeLeft;
    }
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct AttackSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (cooldown, cooldownEnabled) in SystemAPI.Query<RefRW<AttackInCooldown>, EnabledRefRW<AttackInCooldown>>()) {
            if (cooldown.ValueRW.timeLeft > 0f) {
                cooldown.ValueRW.timeLeft -= SystemAPI.Time.DeltaTime;
                if (cooldown.ValueRW.timeLeft <= 0f) {
                    cooldown.ValueRW.timeLeft = 0f;
                    cooldownEnabled.ValueRW = false; // Disable cooldown when time is up
                }
            } else if (cooldownEnabled.ValueRW) {
                cooldown.ValueRW.timeLeft = 0f;
                cooldownEnabled.ValueRW = false;
            }
        }

        var attackJob = new AttackJob {
            enemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
            playerLookup = SystemAPI.GetComponentLookup<PlayerData>(true),
            attackDataLookup = SystemAPI.GetComponentLookup<AttackData>(true),
            cooldownLookup = SystemAPI.GetComponentLookup<AttackInCooldown>(),
            damageBufferLookup = SystemAPI.GetBufferLookup<DamageThisFrame>()
        };

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = attackJob.Schedule(simulation, state.Dependency);
    }

    [Unity.Burst.BurstCompile]
    public struct AttackJob : ICollisionEventsJob {
        [ReadOnly] public ComponentLookup<EnemyTag> enemyLookup;
        [ReadOnly] public ComponentLookup<PlayerData> playerLookup;
        [ReadOnly] public ComponentLookup<AttackData> attackDataLookup;
        public ComponentLookup<AttackInCooldown> cooldownLookup;
        public BufferLookup<DamageThisFrame> damageBufferLookup;


        public void Execute(CollisionEvent collisionEvent) {
            EnemyPlayerCollision(collisionEvent);
        }


        public void EnemyPlayerCollision(CollisionEvent collisionEvent) {
            Entity playerEntity;
            Entity enemyEntity;

            if (playerLookup.HasComponent(collisionEvent.EntityA) && enemyLookup.HasComponent(collisionEvent.EntityB)) {
                playerEntity = collisionEvent.EntityA;
                enemyEntity = collisionEvent.EntityB;
            } else if (playerLookup.HasComponent(collisionEvent.EntityB) && enemyLookup.HasComponent(collisionEvent.EntityA)) {
                playerEntity = collisionEvent.EntityB;
                enemyEntity = collisionEvent.EntityA;
            } else {
                return; // Not a player-enemy collision
            }

            if (cooldownLookup.IsComponentEnabled(enemyEntity)) return;

            var attackData = attackDataLookup[enemyEntity];
            cooldownLookup[enemyEntity] = new AttackInCooldown(attackData.cooldown);
            cooldownLookup.SetComponentEnabled(enemyEntity, true);

            var playerDamageBuffer = damageBufferLookup[playerEntity];
            playerDamageBuffer.Add(new DamageThisFrame { damage = attackData.damage });
        }
    }
}