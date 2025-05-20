using System.Collections.Generic;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;
using Prism.Web.Dto;

namespace Prism.Web
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private List<LoadModelDto> loadModelDtos;

        private bool isLoading;
        
        public async Task LoadModel(string jsonString)
        {
            var data = JsonUtility.FromJson<LoadModelDto>(jsonString);
            var id = data.id;
            var url = data.url;
            var enable = data.enable;
            var properties = data.properties;
            
            await LoadModel(id, url, enable, properties);
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

        public void SetModelProperties(string jsonString)
        {
            var data = JsonUtility.FromJson<SetModelPropertiesDto>(jsonString);
            var id = data.id;
            var properties = data.properties;

            SetModelProperties(id, properties);
        }

        private async Task LoadModel(string id, string url, bool enable, ModelPropertiesDto properties)
        {
            if (isLoading) return;

            isLoading = true;

            var gltfImport = new GltfImport();
            
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
            
            SetModelProperties(id, properties);
            
            isLoading = false;
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

        private void SetModelProperties(string id, ModelPropertiesDto properties)
        {        
            if (!ModelManager.Instance.TryGetModel(id, out var model)) return;
            
            var position = new Vector3(properties.transform.position.x, properties.transform.position.y, properties.transform.position.z);
            var rotation = Quaternion.Euler(properties.transform.rotation.x, properties.transform.rotation.y, properties.transform.rotation.z);
            var scale = new Vector3(properties.transform.scale.x, properties.transform.scale.y, properties.transform.scale.z);
            var shader = properties.shader;
            
            model.transform.SetPositionAndRotation(position, rotation);
            model.transform.localScale = scale;
            
            SetShader(model, shader);
        }

        private void SetShader(GameObject model, string shader)
        {
            if (model.TryGetComponent(out Outline outline) == false)
            {
                outline = model.AddComponent<Outline>();
            }

            switch (shader)
            {
                case "highlight":
                {
                    outline.enabled = true;
                    outline.OutlineMode = Outline.Mode.OutlineAll;
                    outline.OutlineColor = Color.red;
                    outline.OutlineWidth = 5f;
                    
                    break;
                }

                case "none":
                {
                    outline.enabled = false;
                    
                    break;
                }
            }
        }

        private async void Start()
        {
            if (!useInitialSettings) return;

            foreach (var loadModelDto in loadModelDtos)
            {
                var id = loadModelDto.id;
                var url = loadModelDto.url;
                var enable = loadModelDto.enable;
                var properties = loadModelDto.properties;
                
                await LoadModel(id, url, enable, properties);
            }
        }
    }
}