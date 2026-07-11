using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using System.Diagnostics;

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
        var random = new Unity.Mathematics.Random(12345); // Инициализируем генератор случайных чисел

        for (int i = 0; i < spawner.AmountToSpawn; i++)
        {
            // Команда: клонировать сущность-префаб
            Entity newEnemy = ecb.Instantiate(spawner.EnemyPrefab);

            // Генерируем случайную позицию на плоскости
            float3 randomPosition = new float3(
                random.NextFloat(spawner.MinBound.x, spawner.MaxBound.x), 
                0f,
                random.NextFloat(spawner.MinBound.z, spawner.MaxBound.z)
            );
            //UnityEngine.Debug.Log(randomPosition);
            // Задаем позицию через установку компонента LocalTransform
            ecb.SetComponent(newEnemy, LocalTransform.FromPosition(randomPosition));
        }
    }
}
