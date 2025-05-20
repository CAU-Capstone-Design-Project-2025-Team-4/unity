using System.Collections.Generic;
using GLTFast;
using UnityEngine;
using Prism.Web.Dto;

namespace Prism.Web
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private List<LoadModelDto> loadModelDto;

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
        
        public void SetModelProperties(string jsonString)
        {
            var data = JsonUtility.FromJson<SetModelPropertiesDto>(jsonString);
            var id = data.id;
            
            if (!ModelManager.Instance.TryGetModel(id, out var model)) return;
            
            var properties = data.properties;
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
                case "Highlight":
                {
                    outline.enabled = true;
                    outline.OutlineMode = Outline.Mode.OutlineAll;
                    outline.OutlineColor = Color.white;
                    outline.OutlineWidth = 0.1f;
                    
                    break;
                }

                case "None":
                {
                    outline.enabled = false;
                    
                    break;
                }
            }
        }

        private void Start()
        {
            if (!useInitialSettings) return;

            var initialSetModelPropertiesDto = new SetModelPropertiesDto
            {
                id = initialId,
                properties = new ModelPropertiesDto
                {
                    transform = new TransformDto
                    {
                        position = new Vector3Dto
                        {
                            x = initialPosition.x,
                            y = initialPosition.y,
                            z = initialPosition.z
                        },
                        rotation = new Vector3Dto
                        {
                            x = initialRotation.x,
                            y = initialRotation.y,
                            z = initialRotation.z
                        },
                        scale = new Vector3Dto
                        {
                            x = initialScale.x,
                            y = initialScale.y,
                            z = initialScale.z
                        }
                    },
                    shader = initialShader
                }
            };
            
            var jsonString = JsonUtility.ToJson(initialSetModelPropertiesDto);
            
            SetModelProperties(jsonString);
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