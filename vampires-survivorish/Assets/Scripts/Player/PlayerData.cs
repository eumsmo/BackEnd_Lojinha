using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct PlayerData : IComponentData {
    public uint xp;
    public uint coins;

    public PlayerData(uint xp, uint coins) {
        this.xp = xp;
        this.coins = coins;
    }
}

public struct PlayerAttackData : IComponentData {
    public Entity attackPrefab;
    public float attackCooldown;
    public float attackSpeed;
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


public struct PlayerUICanvas : ICleanupComponentData {
    public UnityObjectRef<Transform> canvasTransform;
    public UnityObjectRef<Slider> healthSlider;
}

public struct PlayerUIPrefab : IComponentData {
    public UnityObjectRef<GameObject> prefab;
}