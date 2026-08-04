using UnityEngine;

public class DetectionSystem
{
    private Enemy enemy;
    private float losePlayerTimer;
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
        if (enemy.playerTransform == null) return false;

        Vector2 origin = enemy.transform.position;
        Vector2 toPlayer = (Vector2)enemy.playerTransform.position - origin;
        float distance = toPlayer.magnitude;

        if (distance > enemy.data.playerDetectionRadius) return false;
        if (distance <= enemy.data.hearingRadius) return true;
        if (distance <= 0f) return true;

        RaycastHit2D obstacleHit = Physics2D.CircleCast(
            origin,
            enemy.data.detectionRayThickness,
            toPlayer / distance,
            distance,
            enemy.obstacleLayer
        );
        return obstacleHit.collider == null;
    }

    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }
}