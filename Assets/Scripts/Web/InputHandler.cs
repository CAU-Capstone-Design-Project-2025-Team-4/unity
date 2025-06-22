using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism.Web
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialInputEnabled;
        [SerializeField] private string initialCameraMode;
        
        public void EnableInput(string enable)
        {
            switch (enable)
            {
                case "true":
                    EnableInput();
                    break;
                
                case "false":
                    DisableInput();
                    break;
            }
        }

        public void SetCameraMode(string mode)
        {
            SwitchCurrentPlayerInputActionMap(mode);
        }

        private void Start()
        {
            if (!useInitialSettings) return;
            
            EnableInput(initialInputEnabled);
            SetCameraMode(initialCameraMode);
        }
        
        private void EnableInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif

            playerInput.ActivateInput();
        }

        private void DisableInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif

            playerInput.DeactivateInput();
        }

        private void SwitchCurrentPlayerInputActionMap(string mapNameOrId)
        {
            var inputIsActive = playerInput.inputIsActive ? "true" : "false";
            
            playerInput.SwitchCurrentActionMap(mapNameOrId);
            
            EnableInput(inputIsActive);
        }
    }
}
