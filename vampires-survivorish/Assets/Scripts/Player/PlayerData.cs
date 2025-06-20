using UnityEngine;
using Unity.Entities;

public struct PlayerData : IComponentData {
    public uint health;
    public uint maxHealth;
    public uint xp;
    public uint coins;

    public PlayerData(uint health, uint maxHealth, uint xp, uint coins) {
        this.health = health;
        this.maxHealth = maxHealth;
        this.xp = xp;
        this.coins = coins;
    }
}
