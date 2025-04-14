using UnityEngine;
using GLTFast;

namespace Prism3D
{
    public class ModelLoadHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        
        private GltfImport gltfImport;

        public async void LoadModel(string url)
        {
            gltfImport = new GltfImport();

            bool loadSuccess = await gltfImport.Load(url);

            if (!loadSuccess)
            {
                Debug.LogError("Failed to load glb: " + url);
                return;
            }

            bool instantiateSuccess = await gltfImport.InstantiateMainSceneAsync(parentTransform);

            if (!instantiateSuccess)
            {
                Debug.LogError("Failed to instantiate glb scene");
            }
        }
    }
}