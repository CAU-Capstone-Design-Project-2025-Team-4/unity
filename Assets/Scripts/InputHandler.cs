using UnityEngine;
using UnityEngine.InputSystem;

namespace Prism3D
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;

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

        private void EnableInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = true;
#endif

            playerInput.enabled = true;
        }

        private void DisableInput()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WebGLInput.captureAllKeyboardInput = false;
#endif

            playerInput.enabled = false;
        }

        private void SwitchCurrentPlayerInputActionMap(string mapNameOrId)
        {
            playerInput.SwitchCurrentActionMap(mapNameOrId);
        }
    }
}
