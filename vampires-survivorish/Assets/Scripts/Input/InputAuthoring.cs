using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class InputAuthoring : MonoBehaviour {
    class Baker : Baker<InputAuthoring> {
        public override void Bake(InputAuthoring authoring) {
            InputData inputData = new InputData( new float2(0.0f, 0.0f));
            AddComponent(GetEntity(TransformUsageFlags.None), inputData);
        }
    }
}

public struct InputData : IComponentData {
    public float2 moveDirection;
    public bool spacePressed;

    public InputData(float2 direction) {
        this.moveDirection = direction;
        this.spacePressed = false;
    }
}