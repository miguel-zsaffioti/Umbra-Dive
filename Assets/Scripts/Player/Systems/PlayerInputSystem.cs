using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSystem
{
    private InputActions playerControls;

    public Vector2 MoveInput { get; private set; }

    public PlayerInputSystem()
    {
        playerControls = new InputActions();
    }

    public void Enable()
    {
        playerControls.Player.Enable();
    }

    public void Disable()
    {
        playerControls.Player.Disable();
    }

    public void Tick()
    {
        MoveInput = playerControls.Player.Move.ReadValue<Vector2>();
    }
}
