using UnityEngine;

public class DetectionSystem
{
    private Enemy enemy;
    private float losePlayerTimer; // Cronômetro interno
    private bool isPlayerDetected;

    public DetectionSystem(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void UpdateDetection()
    {
        if (CanSeePlayer())
        {
            isPlayerDetected = true;
            losePlayerTimer = enemy.data.timeToLose;
        }
        else
        {
            losePlayerTimer -= Time.deltaTime;
            if (losePlayerTimer <= 0)
            {
                isPlayerDetected = false;
            }
        }
    }

    private bool CanSeePlayer()
    {
        LayerMask combinedMask = enemy.playerLayer | enemy.obstacleLayer;
        RaycastHit2D hit = Physics2D.CircleCast(
            enemy.transform.position,
            enemy.data.playerDetectionRadius, 
            Vector2.zero,
            0,
            combinedMask
        );

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }
}
