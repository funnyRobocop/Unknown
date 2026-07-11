using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

[BurstCompile]
public partial struct MovementJob : IJobEntity
{
    public float DeltaTime;
    public float ElapsedTime;

    // В параметрах:
    // ref LocalTransform — изменяем позицию
    // ref MovementDirectionComponent — изменяем направление
    // in SpeedComponent — только читаем скорость
    // in IsAliveTag IsUnitTag — фильтр, обрабатываем ТОЛЬКО живых юнитов
    private void Execute(ref LocalTransform transform, ref MovementDirectionComponent direction, in MovementSpeedComponent speed, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        // 1. Рассчитываем новую позицию
        transform.Position += direction.Value * speed.Value * DeltaTime;

        // 2. Добавим хаотичности (псевдо-Random на основе времени и текущей позиции объекта)
        // В Burst нельзя использовать обычный UnityEngine.Random, поэтому используем математический шум.
        float noiseInput = ElapsedTime + transform.Position.x + transform.Position.z;
        float randomX = math.sin(noiseInput * 5.0f);
        float randomZ = math.cos(noiseInput * 1.5f);

        // Плавно подмешиваем случайный вектор к текущему направлению
        float3 randomDir = new float3(randomX, 0, randomZ);
        direction.Value = math.normalize(direction.Value + randomDir * DeltaTime * 0.5f);

        // 3. Разворачиваем куб «лицом» в сторону его движения
        if (math.lengthsq(direction.Value) > 0.001f)
        {
            transform.Rotation = quaternion.LookRotationSafe(direction.Value, math.up());
        }
    }
}