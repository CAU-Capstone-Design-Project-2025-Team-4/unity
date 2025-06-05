using UnityEngine;

namespace Prism
{
    public interface ICamera
    {
        public void OnApply();
        public void OnUpdate();
        public string GetPositionAndRotation();
        public void SetPositionAndRotation(Vector3 position, Vector3 rotation, float interval);
    }
}
