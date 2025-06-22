using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public class EnemyAuthoring : MonoBehaviour {
    public LayerMask collidesWithLayer;

    class Baker : Baker<EnemyAuthoring> {
        public override void Bake(EnemyAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);

            var enemyLayer = LayerMask.NameToLayer("Enemy");
            var enemyLayerMask = (uint) math.pow(2, enemyLayer);

            var collisionFilter = new CollisionFilter {
                BelongsTo = enemyLayerMask,
                CollidesWith = (uint) authoring.collidesWithLayer.value,
            };
            
            AddComponent(entity, new CollisionSetup {
                collisionFilter = collisionFilter
            });
        }
    }
}

public struct EnemyTag : IComponentData {}
