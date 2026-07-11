using Unity.Entities;

// Компонент-сигнал: "этой сущности нужно нанести урон"
// Мы используем DynamicBuffer, чтобы на одну сущность могло прийти несколько ударов за кадр
public struct DamageBufferElement : IBufferElementData
{
    public float Value;
}
