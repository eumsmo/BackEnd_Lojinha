using UnityEngine;
using Unity.Entities;

public class DropAuthoring : MonoBehaviour {
    public GameObject dropPrefab;

    class Baker : Baker<DropAuthoring> {
        public override void Bake(DropAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new DropData {
                dropPrefab = GetEntity(authoring.dropPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct DropData : IComponentData {
    public Entity dropPrefab;
}
