using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Collections;
using Unity.Physics.Systems;

public class XPAuthoring : MonoBehaviour {
    public uint xpAmount;

    class Baker : Baker<XPAuthoring> {
        public override void Bake(XPAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new XPData { xpAmount = authoring.xpAmount });
            AddComponent<DestroyableTag>(entity);
            SetComponentEnabled<DestroyableTag>(entity, false);
        }
    }
}

public struct XPData: IComponentData {
    public uint xpAmount;
}

public struct UpdateXPUI: IComponentData, IEnableableComponent { }


[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
[UpdateBefore(typeof(AfterPhysicsSystemGroup))]
public partial struct XPSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();
    }

    [Unity.Burst.BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var collectJob = new CollectXPJob {
            playerDataLookup = SystemAPI.GetComponentLookup<PlayerData>(),
            xpDataLookup = SystemAPI.GetComponentLookup<XPData>(true),
            destroyableLookup = SystemAPI.GetComponentLookup<DestroyableTag>(),
            updateXPUILookup = SystemAPI.GetComponentLookup<UpdateXPUI>()
        };

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = collectJob.Schedule(simulation, state.Dependency);
    }

    [Unity.Burst.BurstCompile]
    public struct CollectXPJob : ITriggerEventsJob {
        public ComponentLookup<PlayerData> playerDataLookup;
        [ReadOnly] public ComponentLookup<XPData> xpDataLookup;
        public ComponentLookup<DestroyableTag> destroyableLookup;
        public ComponentLookup<UpdateXPUI> updateXPUILookup;

        public void Execute(TriggerEvent triggerEvent) {
            Entity xpEntity;
            Entity playerEntity;

            if (playerDataLookup.HasComponent(triggerEvent.EntityA) && xpDataLookup.HasComponent(triggerEvent.EntityB)) {
                playerEntity = triggerEvent.EntityA;
                xpEntity = triggerEvent.EntityB;
            } else if (playerDataLookup.HasComponent(triggerEvent.EntityB) && xpDataLookup.HasComponent(triggerEvent.EntityA)) {
                playerEntity = triggerEvent.EntityB;
                xpEntity = triggerEvent.EntityA;
            } else {
                return; // Not a valid player-xp collision
            }

            if (destroyableLookup.IsComponentEnabled(xpEntity)) {
                return; // XP already collected
            }

            PlayerData playerData = playerDataLookup[playerEntity];
            XPData xpData = xpDataLookup[xpEntity];

            playerData.xp += xpData.xpAmount;
            playerDataLookup[playerEntity] = playerData;

            destroyableLookup.SetComponentEnabled(xpEntity, true);
            updateXPUILookup.SetComponentEnabled(playerEntity, true);
        }
    }
}

public partial struct UpdateUIXPSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<SimulationSingleton>();
    }

    public void OnUpdate(ref SystemState state) {
        foreach (var (playerData, updateUIXP) in SystemAPI.Query<RefRW<PlayerData>, EnabledRefRW<UpdateXPUI>>()) {
            UIController.Instance.UpdateXP(playerData.ValueRW.xp);
            updateUIXP.ValueRW = false;
        }
    }
}