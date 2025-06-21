using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public partial class InputSystem : SystemBase {
    private Actions actions;

    private EntityManager entityManager;
    private Entity inputEntity;
    private InputData inputData;
    
    protected override void OnCreate() {
        actions = new Actions();
        actions.Enable();
        
        RequireForUpdate<InputData>();
    }

    protected override void OnUpdate() {
        inputEntity = SystemAPI.GetSingletonEntity<InputData>();
        inputData = EntityManager.GetComponentData<InputData>(inputEntity);

        Vector2 moveVector = actions.Game.Move.ReadValue<Vector2>();
        inputData.moveDirection = new float2(moveVector.x, moveVector.y);
        
        bool spacePressed = actions.Game.Space.ReadValue<float>() > 0.5f;
        inputData.spacePressed = spacePressed;
        
        EntityManager.SetComponentData(inputEntity, inputData);
    }
}
