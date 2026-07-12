using UnityEngine;

public class Cooldown
{
    private float duration;
    private float timer;

    public bool IsReady => timer <= 0f;

    public Cooldown(float duration)
    {
        this.duration = duration;
        this.timer = 0f;
    }

    public void Tick(float deltaTime)
    {
        if (timer > 0f)
            timer -= deltaTime;
    }

    public void Reset()
    {
        timer = duration;
    }

    public void SetDuration(float newDuration)
    {
        duration = newDuration;
    }
}