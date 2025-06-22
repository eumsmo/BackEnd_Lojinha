using UnityEngine;
using Unity.Entities;

public class EnemyAuthoring : MonoBehaviour {
    class Baker : Baker<EnemyAuthoring> {
        public override void Bake(EnemyAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);
        }
    }
}

public struct EnemyTag : IComponentData {}
