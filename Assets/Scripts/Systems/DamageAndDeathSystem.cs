using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateAfter(typeof(RegenerationSystem))] // Гарантируем, что урон применяется ПОСЛЕ регенерации
public partial struct DamageAndDeathSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Берем фабрику команд из стандартной группы систем. 
        // EndSimulationEntityCommandBufferSystem выполнит наши команды в самом конце кадра.
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        
        // Создаем сам буфер команд, привязанный к текущему кадру
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

        // Запускаем задачу обработки урона
        var damageJob = new ProcessDamageJob
        {
            Ecb = ecb
        };

        state.Dependency = damageJob.ScheduleParallel(state.Dependency);
    }
}