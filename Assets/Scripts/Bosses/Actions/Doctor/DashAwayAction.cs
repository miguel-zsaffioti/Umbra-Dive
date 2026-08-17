using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "DashAway", story: "Dash away from player", category: "Action", id: "1d2f46bae6e03f927c0e67e18f18765f")]
public partial class DashAwayAction : Action
{

    private Doctor doctor;
    private float dashSpeed = 12f;
    private float dashDuration = 0.25f;

    private enum Phase { DashX, DashY, Done }
    private Phase phase;
    private Vector2 dashDirX;
    private Vector2 dashDirY;
    private float timer;

    protected override Status OnStart()
    {
        doctor = GameObject.GetComponent<Doctor>();
        if (doctor == null || doctor.player == null) return Status.Failure;

        Vector2 currentPos = doctor.rb.position;
        Vector2 playerPos = doctor.player.rb.position;

        float diffX = currentPos.x - playerPos.x;
        float diffY = currentPos.y - playerPos.y;

        dashDirX = new Vector2(Mathf.Sign(diffX == 0 ? 1f : diffX), 0f);
        dashDirY = new Vector2(0f, Mathf.Sign(diffY == 0 ? 1f : diffY));

        bool skipX = Mathf.Abs(diffX) > Mathf.Abs(diffY);

        phase = Phase.DashX;
        timer = 0f;

        doctor.SetMovement(dashDirX, dashSpeed);
        doctor.Dash();

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        timer += Time.deltaTime;

        if (timer >= dashDuration)
        {
            doctor.StopMovement();

            if (phase == Phase.DashX)
            {
                phase = Phase.DashY;
                timer = 0f;
                doctor.SetMovement(dashDirY, dashSpeed);
                return Status.Running;
            }

            phase = Phase.Done;
            return Status.Success;
        }

        doctor.rb.position = doctor.ClampToArena(doctor.rb.position);

        return Status.Running;
    }

    protected override void OnEnd()
    {
        doctor?.StopMovement();
    }
}

