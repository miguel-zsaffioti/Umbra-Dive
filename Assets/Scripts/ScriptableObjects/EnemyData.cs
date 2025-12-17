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

    [Header("Contact Damage")]
    public int damageAmount = 2;
    public float knockbackForce = 20f;

    [Header("Attack Data")]
    public float attackDistance = 2f;
    public float attackDashSpeed = 20f;
    public float damage = 4f;
    public float attackCooldown = 1f;

}
