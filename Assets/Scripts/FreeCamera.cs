using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism3D
{
    public class FreeCamera : MonoBehaviour, ICamera
    {
        [SerializeField] private float lookSpeed;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        private float currentYaw;
        private float currentPitch;
        
        private Vector3 targetPosition;
        
        private Vector2 moveInput;
        private Vector2 lookInput;
        
        public void OnApply()
        {
            SetPositionAndRotation(transform.position, transform.eulerAngles);
        }

        public void OnUpdate()
        {
            UpdateRotation();
            UpdateMovement();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public void SetPositionAndRotation(Vector3 position, Vector3 rotation)
        {
            transform.position = position;
            transform.rotation = Quaternion.Euler(rotation);
            
            currentYaw = rotation.y;
            currentPitch = rotation.x;
        }
        
        private void UpdateRotation()
        {
            currentYaw += lookInput.x * lookSpeed * Time.deltaTime;
            currentPitch -= lookInput.y * lookSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }

        private void UpdateMovement()
        {
            var direction = transform.forward * moveInput.y + transform.right * moveInput.x;

            targetPosition += direction * (moveSpeed * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}