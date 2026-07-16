using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

/*Обычное движение*/
[BurstCompile]
public partial struct MovementJob : IJobEntity
{
    public float DeltaTime;
    public float ElapsedTime;

    private void Execute(ref LocalTransform transform, ref MovementDirectionComponent direction, in MovementSpeedComponent speed, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        // Рассчитываем новую позицию
        transform.Position += direction.Value * speed.Value * DeltaTime;

        // Добавим хаотичности (псевдо-Random на основе времени и текущей позиции объекта)
        // В Burst нельзя использовать обычный UnityEngine.Random, поэтому используем математический шум.
        var noiseInput = ElapsedTime + transform.Position.x + transform.Position.z;

        // Плавно подмешиваем случайный вектор к текущему направлению
        var randomDir = new float3(math.sin(noiseInput), 0f, math.cos(noiseInput));
        direction.Value = math.normalize(direction.Value + randomDir * DeltaTime);

        //Разворачиваем куб «лицом» в сторону его движения
        if (math.lengthsq(direction.Value) > 0.001f)
        {
            transform.Rotation = quaternion.LookRotationSafe(direction.Value, math.up());
        }
    }
}