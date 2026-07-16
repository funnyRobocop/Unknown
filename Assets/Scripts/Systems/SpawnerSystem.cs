using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct SpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Ищем наш спавнер на сцене. Если его нет — ничего не делаем
        if (!SystemAPI.TryGetSingleton(out SpawnerComponent spawner)) return;

        // Отключаем систему сразу после первого кадра, чтобы спавн не шел бесконечно
        state.Enabled = false;

        // Берем ECB для записи отложенных команд создания сущностей
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        // Запускаем цикл спавна. Так как это инициализация, мы можем сделать цикл прямо в системе,
        // но команды Instantiate будут выполняться очень быстро, так как они просто записываются в буфер.
        var random = new Random(12345); // Инициализируем генератор случайных чисел

        for (var i = 0; i < spawner.AmountToSpawn; i++)
        {
            // Команда: клонировать сущность-префаб
            var newEnemy = ecb.Instantiate(spawner.EnemyPrefab);

            // Генерируем случайную позицию на плоскости
            var randomPosition = new float3(
                random.NextFloat(spawner.MinBound.x, spawner.MaxBound.x), 
                random.NextFloat(spawner.MinBound.y, spawner.MaxBound.y), 
                random.NextFloat(spawner.MinBound.z, spawner.MaxBound.z)
            );
            //UnityEngine.Debug.Log(randomPosition);
            // Задаем позицию через установку компонента LocalTransform
            ecb.SetComponent(newEnemy, LocalTransform.FromPosition(randomPosition));
            var randomStartFrame = random.NextFloat(0f, math.TAU);
            ecb.SetComponent(newEnemy, new AnimationFrameComponent { Value = randomStartFrame });
        }
    }
}
