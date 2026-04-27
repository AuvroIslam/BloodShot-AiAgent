using SingletonManager;
using System;
using System.Transactions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Singletons 
{
    public class InputHandler : SingletonPersistent
    {
        public static InputHandler Instance => GetInstance<InputHandler>();

        private InputAction MoveInput;

        public Vector2 MoveDirection { get; private set; }
        public Vector2 MousePosition { get; private set; }


        public event Action<bool> OnFire;

        private void Start()
        {
            MoveInput = gameObject.GetComponent<PlayerInput>().actions.FindAction("Move");
            if (MoveInput == null) { print($"Input System is missing on {gameObject.name}"); }
        }
        private void Update()
        {
            MoveAction();
            LookAction();
        }
        private void MoveAction()
        {
            MoveDirection = MoveInput.ReadValue<Vector2>();
        }

        private void LookAction()
        {
            if (Mouse.current == null) return;
            MousePosition = Mouse.current.position.ReadValue();
        }

        public void FireAction(InputAction.CallbackContext context)
        {
            OnFire?.Invoke(context.performed);
        }
    }
}
