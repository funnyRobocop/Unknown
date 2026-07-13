using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class UnitAuthoring : MonoBehaviour
{
    public float startHealth;
    public float maxHealth;
    public float regenRate;
    public float radiationRate;

    public Vector3 startDir;
    public Vector3 startSpeed;
    public float startAnimationSpeed;

    // Вложенный класс Baker автоматически подхватывается Unity в SubScene
    public class UnitBaker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            // Получаем ссылку на создаваемую сущность
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            // Добавляем компоненты на сущность
            AddComponent(entity, new HealthComponent 
            { 
                Current = authoring.startHealth, 
                Max = authoring.maxHealth 
            });

            AddComponent(entity, new RegenerationComponent 
            { 
                AmountPerSecond = authoring.regenRate 
            });

            AddComponent<IsUnitTag>(entity);
            AddComponent<IsAliveTag>(entity); // На старте юнит живой, добавляем тег
            AddBuffer<DamageBufferElement>(entity); // Инициализируем пустой буфер на сущности    


            AddComponent(entity, new MovementDirectionComponent { Value = authoring.startDir });
            AddComponent(entity, new MovementSpeedComponent { Value = authoring.startSpeed });

            AddComponent(entity, new AnimationFrameComponent { Value = 0f });
            AddComponent(entity, new AnimationSettingsComponent { AnimationSpeed = authoring.startAnimationSpeed });
        }
    }
}
