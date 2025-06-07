using System.Collections;
using Prism.Web.Dto;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism
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

        private bool isTransitioning;
        private Coroutine transitionCoroutine;
        
        private Vector2 moveInput;
        private Vector2 lookInput;
        
        public void OnApply()
        {
            SetPositionAndRotation(transform.position, transform.eulerAngles, 1.0f);
        }

        public void OnUpdate()
        {
            if (!isTransitioning)
            {
                UpdateRotation();
                UpdateMovement();
            }
        }

        public bool IsMoving()
        {
            if (!transform.hasChanged) return false;
            
            transform.hasChanged = false;

            return true;
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            lookInput = context.ReadValue<Vector2>();
        }

        public string GetPositionAndRotation()
        {
            var positionAndRotationDto = PositionAndRotationDto.FromPositionAndRotation(transform.position, transform.eulerAngles);
            var jsonPtr = JsonUtility.ToJson(positionAndRotationDto, false);

            return jsonPtr;
        }

        public void SetPositionAndRotation(Vector3 position, Vector3 rotation, float interval)
        {
            if (interval <= 0f)
            {
                transform.position = position;
                transform.rotation = Quaternion.Euler(rotation);
            
                currentYaw = rotation.y;
                currentPitch = rotation.x;
                targetPosition = position;

                return;
            }

            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            
            transitionCoroutine = StartCoroutine(Transition(position, rotation, interval));
        }

        private IEnumerator Transition(Vector3 targetPosition, Vector3 targetRotation, float interval)
        {
            isTransitioning = true;

            var startPosition = transform.position;
            var startYaw = currentYaw;
            var startPitch = currentPitch;
            
            var targetYaw = targetRotation.y;
            var targetPitch = targetRotation.x;
            
            var yawDiff = Mathf.DeltaAngle(startYaw, targetYaw);
            targetYaw = startYaw + yawDiff;
            
            var elapsedTime = 0f;

            while (elapsedTime < interval)
            {
                elapsedTime += Time.deltaTime;
                
                var t = elapsedTime / interval;
                t = Mathf.SmoothStep(0f, 1f, t);
                
                var currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                transform.position = currentPosition;
                this.targetPosition = currentPosition;
                
                currentYaw = Mathf.Lerp(startYaw, targetYaw, t);
                currentPitch = Mathf.Lerp(startPitch, targetPitch, t);
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
                
                transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
                
                yield return null;
            }
            
            transform.position = targetPosition;
            this.targetPosition = targetPosition;
            currentYaw = targetYaw;
            currentPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
            
            isTransitioning = false;
            transitionCoroutine = null;
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