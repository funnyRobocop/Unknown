using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct AnimationJob : IJobEntity
{
    public float DeltaTime;

    // ref AnimationFrameComponent — меняем текущий кадр для передачи в шейдер
    // in AnimationSettingsComponent — читаем скорость
    // in IsAliveTag — живые зомби анимируются, мертвые (без тега) — застынут в текущем кадре!
    private void Execute(ref AnimationFrameComponent animFrame, in AnimationSettingsComponent settings, in IsAliveTag isAlive)
    {
        // Увеличиваем значение кадра/времени для шейдера
        animFrame.Value += DeltaTime * settings.AnimationSpeed;
        //UnityEngine.Debug.Log($"AnimationJob: animFrame.Value = {animFrame.Value}, DeltaTime = {DeltaTime}, AnimationSpeed = {settings.AnimationSpeed}");
        // Чтобы значение не росло бесконечно, зациклим его (например, по периоду синуса 2*Pi)
        if (animFrame.Value > 6.28318f)
        {
            animFrame.Value -= 6.28318f;
        }
    }
}
