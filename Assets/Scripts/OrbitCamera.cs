using System.Collections;
using Prism.Web.Dto;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism
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
        [SerializeField] private float positionThreshold = 0.01f;
        [SerializeField] private float rotationThreshold = 0.01f;

        private Vector3 prevPosition;
        private Vector3 prevRotation;

        private float currentYaw;
        private float currentPitch;
        private float currentDistance;
        
        private Vector3 targetPosition;

        private bool isTransitioning;
        private Coroutine transitionCoroutine;
        
        private Vector2 lookInput;
        private float zoomInput;
        
        public void OnApply()
        {
            SetPositionAndRotation(transform.position, transform.eulerAngles, 1.0f);
        }

        public void OnUpdate()
        {
            UpdatePrevPositionAndRotation();
            
            if (!isTransitioning)
            {
                UpdateYawAndPitch();
                UpdateDistance();
            }
            
            UpdateTargetPosition();
            UpdatePositionAndRotation();
        }
        
        public bool IsMoving()
        {
            var positionMoved = Vector3.Distance(transform.position, prevPosition) > positionThreshold;
            var rotationMoved = Vector3.Distance(transform.eulerAngles, prevRotation) > rotationThreshold;
            
            return positionMoved || rotationMoved;
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();    
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            zoomInput = context.ReadValue<float>();
        }
        
        public string GetPositionAndRotation()
        {
            var positionAndRotationDto = PositionAndRotationDto.FromPositionAndRotation(transform.position, transform.eulerAngles);
            var jsonPtr = JsonUtility.ToJson(positionAndRotationDto, false);

            return jsonPtr;
        }
        
        public void SetPositionAndRotation(Vector3 position, Vector3 _, float interval)
        {
            var vector = target.position - position;
            var direction = vector.normalized;
            
            var targetDistance = vector.magnitude;
            var targetYaw =  Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            var targetPitch = Mathf.Asin(direction.y) * Mathf.Rad2Deg;

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            
            transitionCoroutine = StartCoroutine(Transition(targetYaw, targetPitch, targetDistance, interval));
        }

        private IEnumerator Transition(float targetYaw, float targetPitch, float targetDistance, float interval)
        {
            isTransitioning = true;

            var startYaw = currentYaw;
            var startPitch = currentPitch;
            var startDistance = currentDistance;
            
            var yawDiff = Mathf.DeltaAngle(startYaw, targetYaw);
            targetYaw = startYaw + yawDiff;
            
            var elapsedTime = 0f;

            while (elapsedTime < interval)
            {
                elapsedTime += Time.deltaTime;
                
                var t = elapsedTime / interval;
                t = Mathf.SmoothStep(0f, 1f, t);
                
                currentYaw = Mathf.Lerp(startYaw, targetYaw, t);
                currentPitch = Mathf.Lerp(startPitch, targetPitch, t);
                currentDistance = Mathf.Lerp(startDistance, targetDistance, t);
                
                currentYaw = Mathf.Repeat(currentYaw, 360f);
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                
                yield return null;
            }
            
            currentYaw = Mathf.Repeat(targetYaw, 360f);
            currentPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
            currentDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            
            isTransitioning = false;
            transitionCoroutine = null;
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
        
        private void UpdatePrevPositionAndRotation()
        {
            prevPosition = transform.position;
            prevRotation = transform.eulerAngles;
        }
    }
}
