using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Entities.UniversalDelegates;

public class PlayerDataAuthoring : MonoBehaviour {
    public uint xp = 0;
    public uint coins = 0;

    public GameObject ataqueBasicoPrefab;
    public float attackCooldown = 0.5f;
    public float attackSpeed = 20f;
    public uint damage = 3;
    public float projectileDuration = 5f;
    public float detectionRange = 5f;

    public GameObject playerUIPrefab;

    class Baker : Baker<PlayerDataAuthoring> {
        public override void Bake(PlayerDataAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            PlayerData playerData = new PlayerData(authoring.xp, authoring.coins);
            AddComponent(entity, playerData);

            // XP UI
            AddComponent<UpdateXPUI>(entity);
            SetComponentEnabled<UpdateXPUI>(entity, false);

            // Health bar
            AddComponent<PlayerUICanvas>(entity);
            AddComponent(entity, new PlayerUIPrefab {
                prefab = authoring.playerUIPrefab
            });

            // Attack
            var enemyLayer = LayerMask.NameToLayer("Enemy");
            var enemyLayerMask = (uint) math.pow(2, enemyLayer);
            var collisionFilter = new CollisionFilter {
                BelongsTo = 1u,
                CollidesWith = enemyLayerMask
            };

            AddComponent(entity, new PlayerAttackData {
                attackPrefab = GetEntity(authoring.ataqueBasicoPrefab, TransformUsageFlags.Dynamic),
                attackCooldown = authoring.attackCooldown,
                attackSpeed = authoring.attackSpeed,
                damage = authoring.damage,
                projectileDuration = authoring.projectileDuration,
                detectionRange = new float3(authoring.detectionRange),
                attackCollisionFilter = collisionFilter
            });
            AddComponent<PlayerAttackCooldown>(entity);
            SetComponentEnabled<PlayerAttackCooldown>(entity, false);
        }
    }
}
