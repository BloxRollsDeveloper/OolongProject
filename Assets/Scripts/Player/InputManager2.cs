using UnityEngine;

public class InputManager2: MonoBehaviour
{
    private InputSystem_Actions _inputSystem;

    public float Horizontal;
    public bool Jump;
    public bool Attack, AttackHeld;
    public bool Drop;

    private void Update()
    {
        Horizontal = _inputSystem.Player.Move.ReadValue<Vector2>().x;
        Jump = _inputSystem.Player.Jump.WasPressedThisFrame();
        Attack = _inputSystem.Player.Interact.WasPressedThisFrame();
        AttackHeld = _inputSystem.Player.Interact.IsPressed();
        Drop = _inputSystem.Player.Drop.WasPressedThisFrame();
    }

    private void Awake() { _inputSystem = new InputSystem_Actions(); }

    private void OnEnable() { _inputSystem.Enable(); }

    private void OnDisable() { _inputSystem.Disable(); }
}