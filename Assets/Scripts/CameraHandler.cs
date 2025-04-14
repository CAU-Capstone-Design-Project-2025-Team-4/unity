using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

namespace Prism3D
{
    public class CameraHandler : MonoBehaviour
    {
        [SerializeField] private OrbitCamera orbitCamera;
        [SerializeField] private FreeCamera freeCamera;
        
        private ICamera currentCamera;
        
        public void SetCameraMode(string mode)
        {
            switch (mode)
            {
                case "Orbit":
                    currentCamera = orbitCamera;
                    break;
                
                case "Free":
                    currentCamera = freeCamera;
                    break;
            }
            
            ApplyCamera();
        }

        public void SetCameraBackgroundMode(string mode)
        {
            if (Camera.main == null) return;
            
            switch (mode)
            {
                case "Skybox" :
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                    break;
                
                case "SolidColor" :
                    Camera.main.clearFlags = CameraClearFlags.SolidColor;
                    break;
            }
        }

        public void SetCameraBackgroundColor(string hex)
        {
            if (!ColorUtility.TryParseHtmlString(hex, out var color)) return;
            if (Camera.main == null) return;
            
            Camera.main.backgroundColor = color;
        }

        private void Update()
        {
            currentCamera?.OnUpdate();
        }

        private void ApplyCamera()
        {
            currentCamera?.OnApply();
        }
    }
}
