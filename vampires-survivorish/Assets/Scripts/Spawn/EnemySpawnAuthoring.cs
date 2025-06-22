using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Random = Unity.Mathematics.Random;

public class EnemySpawnAuthoring : MonoBehaviour {
    public GameObject enemyPrefab;
    public float spawnDistance;
    public float interval;
    public uint randomSeed = 0;

    class Baker : Baker<EnemySpawnAuthoring> {
        public override void Bake(EnemySpawnAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            EnemySpawnData spawnData = new EnemySpawnData(GetEntity(authoring.enemyPrefab,TransformUsageFlags.Dynamic), authoring.spawnDistance, authoring.interval);
            AddComponent(entity, spawnData);

            AddComponent(entity, new InternalEnemySpawnData {
                random = new Random(authoring.randomSeed),
                spawnTimer = 0f
            });
        }
    }
}

public struct EnemySpawnData : IComponentData {
    public Entity enemyPrefab;
    public float spawnDistance;
    public float interval;

    public EnemySpawnData(Entity enemyPrefab, float spawnDistance, float interval) {
        this.enemyPrefab = enemyPrefab;
        this.spawnDistance = spawnDistance;
        this.interval = interval;
    }
}

public struct InternalEnemySpawnData : IComponentData {
    public Random random;
    public float spawnTimer;
}


[BurstCompile]
public partial struct EnemySpawnSystem : ISystem {
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<EnemySpawnData>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {
        var deltaTime = SystemAPI.Time.DeltaTime;

        Entity entity = SystemAPI.GetSingletonEntity<EnemySpawnData>();
        var spawnData = SystemAPI.GetComponent<EnemySpawnData>(entity);
        var internalData = SystemAPI.GetComponent<InternalEnemySpawnData>(entity);

        internalData.spawnTimer -= deltaTime;
        if (internalData.spawnTimer > 0f) {
            SystemAPI.SetComponent(entity, internalData);
            return;
        }

        internalData.spawnTimer = spawnData.interval;

        var playerEntity = SystemAPI.GetSingletonEntity<PlayerData>();
        var playerTransform = SystemAPI.GetComponent<LocalTransform>(playerEntity);

        var angle = internalData.random.NextFloat(0f, math.TAU);
        SystemAPI.SetComponent(entity, internalData);

        var spawnPosition = new float3(
            math.sin(angle),
            math.cos(angle),
            0f
        ) * spawnData.spawnDistance;

        spawnPosition += playerTransform.Position;

        var newEnemy = state.EntityManager.Instantiate(spawnData.enemyPrefab);
        state.EntityManager.SetComponentData(newEnemy, LocalTransform.FromPosition(spawnPosition));
    }
}