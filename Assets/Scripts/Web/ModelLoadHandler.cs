using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using Prism.Web.Dto;

namespace Prism.Web
{
    public class ModelLoadHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialId;
        [SerializeField] private string initialUrl;
        [SerializeField] private bool initialEnable;
        
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
            var properties = data.properties;
            
            if (ModelManager.Instance.ContainsModel(id))
            {
                Debug.LogError("Duplicate model id: " + id);
                
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
            
            ModelManager.Instance.AddModel(id, modelTransform.gameObject);
            
            EnableModel(id, enable);
            
            isLoading = false;
        }

        public void UnloadModel(string id)
        {
            if (!ModelManager.Instance.TryGetModel(id, out var model)) return;

            Destroy(model);
            
            ModelManager.Instance.RemoveModel(id);
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
            if (!ModelManager.Instance.ContainsModel(id)) return;
            
            if (enable)
            {
                EnableModel(ModelManager.Instance.GetModel(id));
            }
            else
            {
                DisableModel(ModelManager.Instance.GetModel(id));
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
            
            var initialLoadModelDto = new LoadModelDto
            {
                id = initialId,
                url = initialUrl,
                enable = initialEnable
            };

            var jsonString = JsonUtility.ToJson(initialLoadModelDto);
            
            LoadModel(jsonString);
        }
    }
}