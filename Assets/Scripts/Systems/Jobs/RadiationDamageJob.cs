using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct RadiationDamageJob : IJobEntity
{
    public EntityCommandBuffer.ParallelWriter Ecb;
    public float DamageAmount;

    // Фильтруем: берем только сущности с IsAliveTag и проверяем наличие DamageBufferElement
    private void Execute(Entity entity, [ChunkIndexInQuery] int entityInQueryIndex, in DynamicBuffer<DamageBufferElement> damageBuffer, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        // Добавляем элемент урона в динамический буфер конкретной сущности
        Ecb.AppendToBuffer(entityInQueryIndex, entity, new DamageBufferElement { Value = DamageAmount });
    }
}
