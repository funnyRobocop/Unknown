using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct HashGridJob : IJobEntity
{
    public NativeParallelMultiHashMap<int, EnemyGridData>.ParallelWriter GridMapWriter;

    private void Execute(Entity entity, in LocalTransform transform, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        var hash = SpatialHashGrid.GetCellHash(transform.Position);
        GridMapWriter.Add(hash, new EnemyGridData { Entity = entity, Position = transform.Position });
    }
}