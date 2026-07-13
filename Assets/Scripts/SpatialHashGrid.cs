using Unity.Mathematics;

public static class SpatialHashGrid
{
    // Размер одной ячейки сетки в метрах. 
    // Должен быть примерно равен диаметру гриба + небольшое расстояние отталкивания
    public const float CellSize = 10.0f; 

    public static int GetCellHash(float3 position)
    {
        int x = (int)math.floor(position.x / CellSize);
        int y = (int)math.floor(position.y / CellSize);
        int z = (int)math.floor(position.z / CellSize);
        
        // Генерируем уникальный хэш-ключ для этой пары координат (простой алгоритм хэширования)
        return (x * 73856093) ^ (y * 48567896) ^ (z * 19349663);
    }
}
