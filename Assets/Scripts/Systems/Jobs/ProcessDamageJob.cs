using Unity.Burst;
using Unity.Entities;


[BurstCompile]
public partial struct ProcessDamageJob : IJobEntity
{
    // Использование AsParallelWriter требует передачи sortKey для безопасной записи из разных потоков
    public EntityCommandBuffer.ParallelWriter Ecb;

    // chunkIndexInQuery — это встроенный параметр Unity, который дает уникальный ID для сортировки команд
    private void Execute(Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery, ref HealthComponent health, DynamicBuffer<DamageBufferElement> damageBuffer, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        if (damageBuffer.IsEmpty) return;

        // Суммируем весь пришедший урон за кадр
        float totalDamage = 0;
        for (int i = 0; i < damageBuffer.Length; i++)
        {
            totalDamage += damageBuffer[i].Value;
        }
        
        // Очищаем буфер для следующего кадра
        damageBuffer.Clear();

        // Применяем урон
        health.Current -= totalDamage;

        // Логика смерти
        if (health.Current <= 0)
        {
            health.Current = 0;

            // Записываем команды в ECB. Они выполнятся чуть позже в главном потоке.
            Ecb.RemoveComponent<IsAliveTag>(chunkIndexInQuery, entity);
            Ecb.AddComponent<IsDeadTag>(chunkIndexInQuery, entity);
            
            // удаление компонентов которые мертвым не нужны
            Ecb.RemoveComponent<RegenerationComponent>(chunkIndexInQuery, entity);
            Ecb.RemoveComponent<MovementDirectionComponent>(chunkIndexInQuery, entity);
            Ecb.RemoveComponent<MovementSpeedComponent>(chunkIndexInQuery, entity);
        }
    }
}
