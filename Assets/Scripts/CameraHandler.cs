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
                case "skybox" :
                    Camera.main.clearFlags = CameraClearFlags.Skybox;
                    break;
                
                case "solid" :
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

        public void SetCameraPositionAndRotation(string jsonString)
        {
            var data = JsonUtility.FromJson<PositionAndRotationData>(jsonString);
            var position = new Vector3(data.position.x, data.position.y, data.position.z);
            var rotation = new Vector3(data.rotation.x, data.rotation.y, data.rotation.z);
            
            currentCamera?.SetPositionAndRotation(position, rotation);
        }

        private void Update()
        {
            currentCamera?.OnUpdate();
        }

        private void ApplyCamera()
        {
            currentCamera?.OnApply();
        }

        [System.Serializable]
        private class PositionAndRotationData
        {
            public Vector3Data position;
            public Vector3Data rotation;
        }

        [System.Serializable]
        private class Vector3Data
        {
            public float x;
            public float y;
            public float z;
        }
    }
}
