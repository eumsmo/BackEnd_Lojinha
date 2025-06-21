using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

public class AnimationAuthoring : MonoBehaviour {
    class Baker : Baker<AnimationAuthoring> {
        public override void Bake(AnimationAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new FacingDirectionData { direction = 1.0f });
        }
    }
}

[MaterialProperty("_Facing")]
public struct FacingDirectionData : IComponentData {
    public float direction;
    public FacingDirectionData(float direction) {
        this.direction = direction;
    }
}
