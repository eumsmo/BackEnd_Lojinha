using UnityEngine;
using Unity.Entities;

public class PlayerDataAuthoring : MonoBehaviour {
    public uint health = 100;
    public uint maxHealth = 100;
    public uint xp = 0;
    public uint coins = 0;

    class Baker : Baker<PlayerDataAuthoring> {
        public override void Bake(PlayerDataAuthoring authoring) {
            PlayerData playerData = new PlayerData(authoring.health, authoring.maxHealth, authoring.xp, authoring.coins);
            AddComponent(GetEntity(TransformUsageFlags.Dynamic), playerData);
        }
    }
}
