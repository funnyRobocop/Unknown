using Unity.Entities;
using Unity.Rendering; // Нужен для атрибута MaterialProperty

// Атрибут связывает этот ECS-компонент со свойством шейдера _AnimationFrame
[MaterialProperty("_AnimationFrame")]
public struct AnimationFrameComponent : IComponentData
{
    public float Value;
}
