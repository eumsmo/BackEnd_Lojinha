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

public struct PlayerUICanvas : ICleanupComponentData {
    public UnityObjectRef<Transform> canvasTransform;
    public UnityObjectRef<Slider> healthSlider;
}

public struct PlayerUIPrefab : IComponentData {
    public UnityObjectRef<GameObject> prefab;
}