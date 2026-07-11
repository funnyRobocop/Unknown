using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickDamage : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _livingUnitsQuery;

    public float damage = 40f;

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

            Debug.Log($"[Player] Наносим урон. Всего живых сущностей на сцене: {entities.Length}");

            // Так как мы находимся в главном потоке MonoBehaviour, мы можем писать в EntityManager напрямую
            for (int i = 0; i < entities.Length; i++)
            {
                var  buffer = _entityManager.GetBuffer<DamageBufferElement>(entities[i]);
                
                // Наносим урон
                buffer.Add(new DamageBufferElement { Value = damage });
            }
        }
    }
}
