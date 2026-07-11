using Unity.Entities;
using Unity.Mathematics;

public struct SpawnerComponent : IComponentData
{
    public Entity EnemyPrefab; // Ссылка на ECS-префаб
    public int AmountToSpawn;   // Сколько штук спавнить
    public float3 MinBound;
    public float3 MaxBound;
}
