using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using Prism.Dto;

namespace Prism
{
    public class ModelLoader : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private ModelManager modelManager;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialUrl;
        
        private bool isLoading;
        
        public async void LoadModel(string jsonString)
        {
            if (isLoading) return;
            
            isLoading = true;

            var gltfImport = new GltfImport();
            var data = JsonUtility.FromJson<LoadModelDto>(jsonString);
            var id = data.id;
            var url = data.url;
            var enable = data.enable;
            
            if (modelManager.ContainsModel(id))
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
            
            modelManager.AddModel(id, modelTransform.gameObject);
            
            EnableModel(id, enable);
            
            isLoading = false;
        }

        public void UnloadModel(string id)
        {
            if (!modelManager.TryGetModel(id, out var model)) return;

            Destroy(model);
            
            modelManager.RemoveModel(id);
        }

        public void EnableModel(string jsonString)
        {
            var data = JsonUtility.FromJson<EnableModelDto>(jsonString);
            var id = data.id;
            var enable = data.enable;
            
            EnableModel(id, enable);
        }

        private void EnableModel(string id, bool enable)
        {
            if (!modelManager.ContainsModel(id)) return;
            
            if (enable)
            {
                EnableModel(modelManager.GetModel(id));
            }
            else
            {
                DisableModel(modelManager.GetModel(id));
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