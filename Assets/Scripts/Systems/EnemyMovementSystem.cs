using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct EnemyMovementSystem : ISystem
{
     // Создаем нативный контейнер для хранения сетки. 
    // MultiHashMap позволяет хранить несколько сущностей (грибов) в одной ячейке (хэше)
    private NativeParallelMultiHashMap<int, EnemyGridData> _gridMap;

    public void OnCreate(ref SystemState state)
    {
        // Инициализируем контейнер в Persistent памяти
        _gridMap = new NativeParallelMultiHashMap<int, EnemyGridData>(100000, Allocator.Persistent);
    }

    public void OnDestroy(ref SystemState state)
    {
        // Обязательно освобождаем память при уничтожении системы (профилактика утечек памяти)
        if (_gridMap.IsCreated) _gridMap.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // Сбрасываем и подготавливаем контейнер под текущее количество живых объектов
        _gridMap.Clear();
        
        // Получаем запрос на всех живых 
        var enemyQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, IsAliveTag, IsUnitTag>().Build();
        int enemyCount = enemyQuery.CalculateEntityCount();
        
        if (enemyCount == 0) return;
        
        // Выделяем необходимую емкость
        //_gridMap.Capacity = enemyCount;

        // Заполнение сетки.
        // Используем ParallelWriter, чтобы сотни потоков могли безопасно одновременно писать данные
        var hashGridJob = new HashGridJob
        {
            GridMapWriter = _gridMap.AsParallelWriter()
        };
        state.Dependency = hashGridJob.ScheduleParallel(state.Dependency);

        // Движение + Separation (Отталкивание).
        // Передаем саму заполненную сетку внутрь джобы движения
        var movementJob = new AdvancedMovementJob
        {
            GridMap = _gridMap,
            DeltaTime = SystemAPI.Time.DeltaTime,
            ElapsedTime = (float)SystemAPI.Time.ElapsedTime
        };
        // ВАЖНО: ScheduleParallel автоматически создаст барьеры безопасности (Read/Write) для _gridMap
        state.Dependency = movementJob.ScheduleParallel(state.Dependency);
    }
}