using UnityEngine;
using GLTFast;

namespace Prism3D
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialUrl;

        private GameObject loadedModel;
        
        private bool isLoading = false;
        
        public async void LoadModel(string url)
        {
            if (isLoading) return;
            
            isLoading = true;
            
            var gltfImport = new GltfImport();

            if (!await gltfImport.Load(url))
            {
                Debug.LogError("Failed to load glb: " + url);
                isLoading = false;
                
                return;
            }

            UnloadModel();
            
            var modelTransform = new GameObject("Loaded Model").transform;

            modelTransform.SetParent(parentTransform);

            if (!await gltfImport.InstantiateMainSceneAsync(modelTransform))
            {
                Debug.LogError("Failed to instantiate glb scene");
                isLoading = false;
                
                return;
            }
            
            loadedModel = modelTransform.gameObject;
            isLoading = false;
        }

        public void UnloadModel()
        {
            if (loadedModel == null) return;
            
            Destroy(loadedModel);
            loadedModel = null;
        }

        private void Start()
        {
            if (!useInitialSettings) return;
            
            LoadModel(initialUrl);
        }
    }
}