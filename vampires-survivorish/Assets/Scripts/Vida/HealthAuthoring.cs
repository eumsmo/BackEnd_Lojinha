using UnityEngine;
using Unity.Entities;

public class HealthAuthoring : MonoBehaviour {
    public uint maxHealth = 100;

    class Baker : Baker<HealthAuthoring> {
        public override void Bake(HealthAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            HealthData healthData = new HealthData(authoring.maxHealth);
            AddComponent(entity, healthData);
            AddBuffer<DamageThisFrame>(entity);

            AddComponent<DestroyableTag>(entity);
            SetComponentEnabled<DestroyableTag>(entity, false);
        }
    }
}

public struct HealthData : IComponentData {
    public uint health;
    public uint maxHealth;

    public HealthData(uint maxHealth) {
        this.health = maxHealth;
        this.maxHealth = maxHealth;
    }
}

public struct DamageThisFrame : IBufferElementData {
    public uint damage;
}

public partial struct ApplyDamageSystem : ISystem {
    
    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        foreach (var (health, damage, entity) in SystemAPI.Query<RefRW<HealthData>, DynamicBuffer<DamageThisFrame>>().WithEntityAccess()) {
            if (damage.IsEmpty) continue;
            uint totalDamage = 0;

            foreach (var d in damage) {
                totalDamage += d.damage;
            }

            damage.Clear();

            if (totalDamage >= health.ValueRO.health) {
                totalDamage = 0;
                state.EntityManager.SetComponentEnabled<DestroyableTag>(entity, true);
            } else {
                health.ValueRW.health -= totalDamage;
            }
        }
    }

}
