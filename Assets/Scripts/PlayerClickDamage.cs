using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickDamage : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _livingUnitsQuery;

    public float damage;

    void Start()
    {
        // Мир ECS создается автоматически. Получаем доступ к менеджеру сущностей
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Создаем запрос, который найдет всех живых юнитов на сцене
        _livingUnitsQuery = _entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<IsAliveTag>(),
            ComponentType.ReadWrite<DamageBufferElement>()
        );
    }

    void Update()
    {
        // При нажатии на пробел или левую кнопку мыши наносим урон
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            // Получаем нативный массив всех сущностей, подходящих под наш запрос прямо сейчас
            using var entities = _livingUnitsQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);

            // Так как мы находимся в главном потоке MonoBehaviour, мы можем писать в EntityManager напрямую
            foreach (var entity in entities)
            {
                var  buffer = _entityManager.GetBuffer<DamageBufferElement>(entity);
                
                // Наносим урон
                buffer.Add(new DamageBufferElement { Value = damage });
            }
        }
    }
}
