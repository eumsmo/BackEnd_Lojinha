using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class MovementAuthoring : MonoBehaviour {
    public float speed = 1.0f;

    class Baker : Baker<MovementAuthoring> {
        public override void Bake(MovementAuthoring authoring) {
            MovementData movementData = new MovementData(authoring.speed, new float2(1.0f, 0.0f));
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), movementData);
        }
    }
}

public struct MovementData : IComponentData {
    public float speed;
    public float2 direction;

    public MovementData(float speed, float2 direction) {
        this.speed = speed;
        this.direction = direction;
    }
}