using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism3D
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialInputEnabled;
        [SerializeField] private string initialKeyboardInputEnabled;
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

        public void EnableKeyboardInput(string enable)
        {
            switch (enable)
            {
                case "true":
                    EnableKeyboardInput();
                    break;
                
                case "false":
                    DisableKeyboardInput();
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
            EnableKeyboardInput(initialKeyboardInputEnabled);
            SetCameraMode(initialCameraMode);
        }
        
        private void EnableInput()
        {
            playerInput.ActivateInput();
        }

        private void DisableInput()
        {
            playerInput.DeactivateInput();
        }

        private void EnableKeyboardInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif
            
        }

        private void DisableKeyboardInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif
            
        }
        
        private void SwitchCurrentPlayerInputActionMap(string mapNameOrId)
        {
            playerInput.SwitchCurrentActionMap(mapNameOrId);
        }
    }
}
