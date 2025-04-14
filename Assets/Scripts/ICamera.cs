using UnityEngine;

namespace Prism3D
{
    public interface ICamera
    {
        public void OnApply();
        public void OnUpdate();
        public void SetPositionAndRotation(Vector3 position, Vector3 rotation);
    }
}
