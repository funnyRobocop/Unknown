using Unity.Burst;
using Unity.Entities;

[BurstCompile]
[UpdateBefore(typeof(DamageAndDeathSystem))] 
public partial struct RegenerationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Создаем задачу (Job), передаем туда deltaTime
        var regenJob = new RegenerationJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        // Запускаем задачу параллельно на всех потоках для всех подходящих сущностей. 
        // state.Dependency гарантирует правильный порядок выполнения систем (профилактика Race Conditions)
        state.Dependency = regenJob.ScheduleParallel(state.Dependency);
    }
}
