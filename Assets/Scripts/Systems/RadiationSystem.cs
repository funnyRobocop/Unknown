using Unity.Burst;
using Unity.Entities;

[BurstCompile]
// Эта система должна отработать ДО того, как DamageAndDeathSystem очистит и применит буферы
[UpdateBefore(typeof(DamageAndDeathSystem))] 
public partial struct RadiationSystem : ISystem
{
    private float _timer;

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _timer += SystemAPI.Time.DeltaTime;

        // Наносим урон только раз в секунду
        if (_timer >= 1.0f)
        {
            _timer = 0f;

            //для безопасного добавления элементов в буфер из потоков нужен ECB
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter();

            var radiationJob = new RadiationDamageJob
            {
                Ecb = ecb,
                DamageAmount = 15f // Базовый урон от радиации
            };

            state.Dependency = radiationJob.ScheduleParallel(state.Dependency);
        }
    }
}