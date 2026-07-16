using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;


[BurstCompile]
public partial struct AdvancedMovementJob : IJobEntity
{
    [ReadOnly] public NativeParallelMultiHashMap<int, EnemyGridData> GridMap;
    public float DeltaTime;
    public float ElapsedTime;

    private void Execute(Entity entity, ref LocalTransform transform, ref MovementDirectionComponent direction, in MovementSpeedComponent speed, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        // --- БЛОК SEPARATION (ОТТАЛКИВАНИЕ) ---
        var separationForce = float3.zero;
        var neighborsCount = 0;
        var separationRadius = 5f; // Расстояние, ближе которого грибы начнут толкаться

        // Находим хэш текущей ячейки, где стоит наш гриб
        var currentHash = SpatialHashGrid.GetCellHash(transform.Position);

        // Проверяем текущую ячейку сетки на наличие соседей
        if (GridMap.TryGetFirstValue(currentHash, out EnemyGridData neighbor, out var iterator))
        {
            do
            {
                // Самого себя толкать не нужно
                if (neighbor.Entity == entity) continue;

                // Считаем дистанцию до соседа
                var distance = math.distance(transform.Position, neighbor.Position);

                if (distance < separationRadius && distance > 0.001f)
                {
                    // Вектор направления "от соседа к нам"
                    var pushDir = math.normalize(transform.Position - neighbor.Position);
                    
                    // Чем ближе сосед, тем сильнее сила выталкивания (линейное затухание)
                    separationForce += pushDir * (1.0f - (distance / separationRadius));
                    neighborsCount++;
                }
            } 
            while (GridMap.TryGetNextValue(out neighbor, ref iterator));
        }

        // --- БЛОК ДВИЖЕНИЯ ---
        // Если рядом есть соседи, подмешиваем силу отталкивания к нашему базовому направлению движения
        if (neighborsCount > 0)
        {
            // Плавно корректируем вектор направления, смещая его в сторону свободного места
            direction.Value = math.normalize(direction.Value + separationForce * DeltaTime);
        }

        // Применяем хаотичный шум 
        var noiseInput = ElapsedTime + transform.Position.x + transform.Position.z;
        var randomDir = new float3(math.sin(noiseInput), 0f, math.cos(noiseInput));
        direction.Value = math.normalize(direction.Value + randomDir * DeltaTime);

        // Перемещаем объект
        transform.Position += direction.Value * speed.Value * DeltaTime;

        // Поворачиваем по вектору движения
        if (math.lengthsq(direction.Value) > 0.001f)
        {
            transform.Rotation = quaternion.LookRotationSafe(direction.Value, math.up());
        }
    }
}