using Unity.Burst;
using Unity.Entities;

// Сама задача, которая будет выполняться на воркер-потоках
[BurstCompile]
public partial struct RegenerationJob : IJobEntity
{
    public float DeltaTime;

    // Метод Execute автоматически генерирует Query на основе аргументов.
    // RefRW (Read-Write) — данные будут изменяться.
    // RefRO (Read-Only) — данные только для чтения (позволяет Unity параллелить этот Job с другими RO)
    // inside `in` параметр IsAliveTag выступает в роли фильтра: обрабатываем только живых
    private void Execute(ref HealthComponent health, in RegenerationComponent regen, in IsAliveTag isAlive, in IsUnitTag isUnit)
    {
        if (health.Current < health.Max)
        {
            health.Current += regen.AmountPerSecond * DeltaTime;
            
            // Защита от переполнения
            if (health.Current > health.Max)
            {
                health.Current = health.Max;
            }
        }
    }
}