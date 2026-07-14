using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Важно: подключаем пространство имен новой Input System
using TMPro;
// Подключаем пространства имен нативной физики
using Unity.Physics; 
using Unity.Physics.Systems; 

public class GameUIManager : MonoBehaviour
{
    [Header("Global UI")]
    [SerializeField] private TextMeshProUGUI totalAliveText;

    [Header("Target UI")]
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private Image targetHealthBarFill;
    [SerializeField] private TextMeshProUGUI targetNameText;

    private EntityManager _entityManager;
    private EntityQuery _aliveZombiesQuery;
    private Entity _selectedEntity = Entity.Null;

    void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _aliveZombiesQuery = _entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<IsUnitTag>(),
            ComponentType.ReadOnly<IsAliveTag>()
        );
    }

    void Update()
    {
        int aliveCount = _aliveZombiesQuery.CalculateEntityCount();
        totalAliveText.text = $"Alive Units: {aliveCount}";

        HandleMouseClick();

        UpdateTargetHealth();
    }

private void HandleMouseClick()
{
    var pointer = Pointer.current;
    if (pointer == null) return;

    if (pointer.press.wasPressedThisFrame)
    {
        Vector2 screenPosition = pointer.position.ReadValue();
        UnityEngine.Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // 1. Создаем быстрый временный запрос на синглтон физического мира
        using EntityQuery physicsQuery = _entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
        
        // Безопасная проверка: если физика на этом кадре еще не построилась, выходим
        if (physicsQuery.IsEmptyIgnoreFilter) return;

        // Извлекаем синглтон напрямую из EntityManager
        PhysicsWorldSingleton physicsWorldSingleton = physicsQuery.GetSingleton<PhysicsWorldSingleton>();
        CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

        // 2. Формируем данные для луча
        RaycastInput input = new RaycastInput
        {
            Start = ray.origin,
            End = ray.origin + ray.direction * 100f, 
            Filter = CollisionFilter.Default 
        };

        // 3. Выполняем нативный CastRay
        if (collisionWorld.CastRay(input, out Unity.Physics.RaycastHit hit))
        {
            // Получаем Entity сущности, в которую попал луч
            _selectedEntity = hit.Entity; 
            
            targetPanel.SetActive(true);
            targetNameText.text = $"Mushroom (ID: {_selectedEntity.Index})";
            
            Debug.Log($"[DOTS Physics] Успешно попали в Entity ID: {_selectedEntity.Index}");
        }
        else
        {
            Debug.Log("[DOTS Physics] Промах. Ни один нативный коллайдер не задет.");
        }
    }
}

    private void UpdateTargetHealth()
    {
        if (_selectedEntity == Entity.Null) return;

        if (!_entityManager.Exists(_selectedEntity))
        {
            _selectedEntity = Entity.Null;
            targetPanel.SetActive(false);
            return;
        }

        if (_entityManager.HasComponent<HealthComponent>(_selectedEntity))
        {
            HealthComponent health = _entityManager.GetComponentData<HealthComponent>(_selectedEntity);
            
            if (health.Current <= 0)
            {
                targetHealthBarFill.fillAmount = 0;
                targetNameText.text = "DEAD";
                return;
            }

            targetHealthBarFill.fillAmount = health.Current / health.Max;
        }
    }
}
