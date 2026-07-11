using Unity.Entities;

public struct HealthComponent : IComponentData
{
    public float Current;
    public float Max;
}