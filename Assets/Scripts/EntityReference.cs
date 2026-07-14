using Unity.Entities;
using UnityEngine;

// Этот скрипт вешается на префаб, чтобы MonoBehaviour-рейкаст мог узнать Entity объекта
public class EntityReference : MonoBehaviour
{
    public Entity LinkedEntity;
}
