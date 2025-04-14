using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism3D
{
    public class OrbitCamera : MonoBehaviour, ICamera
    {
        [SerializeField] private Transform target;

        [SerializeField] private float lookSpeed;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float smoothSpeed;
        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;
        [SerializeField] private float minPitch;
        [SerializeField] private float maxPitch;

        private float currentYaw;
        private float currentPitch;
        private float currentDistance;
        
        private Vector3 targetPosition;

        private Vector2 lookInput;
        private float zoomInput;
        
        public void OnApply()
        {
            SetPositionAndRotation(transform.position, transform.eulerAngles);
        }

        public void OnUpdate()
        {
            UpdateYawAndPitch();
            UpdateDistance();
            UpdateTargetPosition();
            UpdatePositionAndRotation();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();    
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            zoomInput = context.ReadValue<float>();
        }
        
        public void SetPositionAndRotation(Vector3 position, Vector3 _)
        {
            var vector = target.position - position;
            var direction = vector.normalized;

            currentDistance = vector.magnitude;
            currentYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            currentPitch = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        }
        
        private void UpdateYawAndPitch()
        {
            currentYaw += lookInput.x * lookSpeed * Time.deltaTime;
            currentYaw = Mathf.Repeat(currentYaw, 360);

            currentPitch -= lookInput.y * lookSpeed * Time.deltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }
        
        private void UpdateDistance()
        {
            currentDistance -= zoomInput * zoomSpeed * Time.deltaTime;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }

        private void UpdateTargetPosition()
        {
            var rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
            var offset = rotation * Vector3.back * currentDistance;
            
            targetPosition = target.position + offset;
        }

        private void UpdatePositionAndRotation()
        {
            transform.position = Vector3.Slerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target);
        }
    }
}
