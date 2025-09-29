using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Data/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float stopDistance = 0.6f;

    [Header("Detection")]
    public float playerDetectionRadius = 5f;
    public float timeToLose = 2f;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackDamage = 10f;
    public float attackCooldown = 2f;
}
