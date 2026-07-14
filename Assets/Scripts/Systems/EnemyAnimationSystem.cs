using Unity.Burst;
using Unity.Entities;

//[UpdateInGroup(typeof(PresentationSystemGroup))]
[BurstCompile]
public partial struct EnemyAnimationSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var animationJob = new AnimationJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime
        };

        // Традиционно параллелим задачу на все ядра
        state.Dependency = animationJob.ScheduleParallel(state.Dependency);
    }
}
