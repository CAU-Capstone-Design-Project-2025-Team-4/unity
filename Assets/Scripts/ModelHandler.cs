using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using Prism.Dto;

namespace Prism
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialUrl;

        private readonly Dictionary<string, GameObject> loadedModels = new Dictionary<string, GameObject>();

        private bool isLoading = false;
        
        public async void LoadModel(string jsonString)
        {
            if (isLoading) return;
            
            isLoading = true;

            var gltfImport = new GltfImport();
            var data = JsonUtility.FromJson<LoadModelDto>(jsonString);
            var id = data.id;
            var url = data.url;
            var enable = data.enable;
            
            if (loadedModels.ContainsKey(id))
            {
                Debug.LogError("Duplicate model: " + id);
                
                isLoading = false;
                
                return;
            }

            if (!await gltfImport.Load(url))
            {
                Debug.LogError("Failed to load glb: " + url);
                
                isLoading = false;
                
                return;
            }

            var modelTransform = new GameObject(id).transform;

            modelTransform.SetParent(parentTransform);

            if (!await gltfImport.InstantiateMainSceneAsync(modelTransform))
            {
                Debug.LogError("Failed to instantiate glb scene");
                
                Destroy(modelTransform.gameObject);
                
                isLoading = false;
                
                return;
            }
            
            loadedModels.Add(id, modelTransform.gameObject);
            
            EnableModel(id, enable);
            
            isLoading = false;
        }

        public void UnloadModel(string id)
        {
            if (!loadedModels.TryGetValue(id, out var model)) return;

            Destroy(model);
            
            loadedModels.Remove(id);
        }

        public void EnableModel(string jsonString)
        {
            var data = JsonUtility.FromJson<EnableModelDto>(jsonString);
            var id = data.id;
            var enable = data.enable;
            
            EnableModel(id, enable);
        }

        public void SetModelProperties(string jsonString)
        {
            var data = JsonUtility.FromJson<>()
        }

        private void EnableModel(string id, bool enable)
        {
            if (!loadedModels.ContainsKey(id)) return;
            
            if (enable)
            {
                EnableModel(loadedModels[id]);
            }
            else
            {
                DisableModel(loadedModels[id]);
            }
        }
        
        private void EnableModel(GameObject model)
        {
            model.SetActive(true);
        }

        private void DisableModel(GameObject model)
        {
            model.SetActive(false);
        }

        private void Start()
        {
            if (!useInitialSettings) return;
            
            LoadModel(initialUrl);
        }
    }
}