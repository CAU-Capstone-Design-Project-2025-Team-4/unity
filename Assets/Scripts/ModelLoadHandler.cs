using UnityEngine;
using GLTFast;

namespace Prism3D
{
    public class ModelLoadHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialUrl;
        
        private GltfImport gltfImport;

        public async void LoadModel(string url)
        {
            gltfImport = new GltfImport();

            var loadSuccess = await gltfImport.Load(url);

            if (!loadSuccess)
            {
                Debug.LogError("Failed to load glb: " + url);
                return;
            }

            var instantiateSuccess = await gltfImport.InstantiateMainSceneAsync(parentTransform);

            if (!instantiateSuccess)
            {
                Debug.LogError("Failed to instantiate glb scene");
            }
        }

        private void Start()
        {
            if (!useInitialSettings) return;
            
            LoadModel(initialUrl);
        }
    }
}