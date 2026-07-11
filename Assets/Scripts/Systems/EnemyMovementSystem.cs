using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Передаем в задачу deltaTime и текущее время для генерации псевдослучайного шума
        var movementJob = new MovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            ElapsedTime = (float)SystemAPI.Time.ElapsedTime
        };

        // Запускаем задачу параллельно на всех потоках для всех подходящих сущностей. 
        // state.Dependency гарантирует правильный порядок выполнения систем (профилактика Race Conditions)
        state.Dependency = movementJob.ScheduleParallel(state.Dependency);
    }
}