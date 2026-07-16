using Unity.Entities;
using UnityEngine;

public class SpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int amountToSpawn;
    public Vector3 minBound;
    public Vector3 maxBound;

    public class SpawnerBaker : Baker<SpawnerAuthoring>
    {
        public override void Bake(SpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpawnerComponent
            {
                // конвертировуем GameObject-префаб в ECS-сущность
                EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                AmountToSpawn = authoring.amountToSpawn,
                MinBound = authoring.minBound,
                MaxBound = authoring.maxBound
            });
        }
    }
}
