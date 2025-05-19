using UnityEngine;

namespace Prism.Web
{
    public interface ICamera
    {
        public void OnApply();
        public void OnUpdate();
        public void SetPositionAndRotation(Vector3 position, Vector3 rotation);
    }
}
