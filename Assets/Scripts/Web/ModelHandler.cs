using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GLTFast;
using UnityEngine;
using Prism.Web.Dto;

namespace Prism.Web
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private Transform parentTransform;
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private List<LoadModelDto> initialLoadModelDtos;

        [DllImport("__Internal")]
        private static extern void ModelLoadCallback(string idPtr);
        
        private readonly Queue<LoadModelDto> loadModelQueue = new();
        private bool isProcessingLoadModelQueue;
        
        public void LoadModel(string jsonString)
        {
            var data = JsonUtility.FromJson<LoadModelDto>(jsonString);

            loadModelQueue.Enqueue(data);

            if (!isProcessingLoadModelQueue)
            {
                StartCoroutine(ProcessLoadModelQueue());
            }
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

        private IEnumerator ProcessLoadModelQueue()
        {
            isProcessingLoadModelQueue = true;

            while (loadModelQueue.Count > 0)
            {
                var data = loadModelQueue.Dequeue();
                
                var id = data.id;
                var url = data.url;
                var enable = data.enable;
                var properties = data.properties;
                
                if (ModelManager.Instance.ContainsModel(id))
                {
                    Debug.LogError("Duplicate model id: " + id);

                    continue;
                }

                var gltfImport = new GltfImport();

                var loadSuccess = false;
                
                yield return StartCoroutine(LoadGltfAsync(gltfImport, url, success => loadSuccess = success));
                
                if (!loadSuccess)
                {
                    Debug.LogError("Failed to load glb: " + url);

                    continue;
                }

                var modelTransform = new GameObject(id).transform;

                modelTransform.SetParent(parentTransform);

                var instantiateSuccess = false;
                
                yield return StartCoroutine(InstantiateGltfAsync(gltfImport, modelTransform, success => instantiateSuccess = success));
                
                if (!instantiateSuccess)
                {
                    Debug.LogError("Failed to instantiate glb scene");
                
                    Destroy(modelTransform.gameObject);

                    continue;
                }
            
                ModelManager.Instance.AddModel(id, modelTransform.gameObject);
            
                EnableModel(id, enable);
            
                SetModelProperties(id, properties);
                
#if UNITY_WEBGL && !UNITY_EDITOR
                ModelLoadCallback(id);
#endif

                yield return null;
            }
            
            isProcessingLoadModelQueue = false;
        }

        private IEnumerator LoadGltfAsync(GltfImport gltfImport, string url, Action<bool> callback)
        {
            var task = gltfImport.Load(url);

            while (!task.IsCompleted)
            {
                yield return null;
            }

            callback(task.Result);
        }
        
        private IEnumerator InstantiateGltfAsync(GltfImport gltfImport, Transform parent, Action<bool> callback)
        {
            var task = gltfImport.InstantiateMainSceneAsync(parent);
    
            while (!task.IsCompleted)
            {
                yield return null;
            }
            
            callback(task.Result);
        }
        
        private void EnableModel(string id, bool enable)
        {
            if (!ModelManager.Instance.TryGetModel(id, out var model)) return;
            
            if (enable)
            {
                EnableModel(model);
            }
            else
            {
                DisableModel(model);
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

        private void Start()
        {
            if (!useInitialSettings) return;

            foreach (var loadModelDto in initialLoadModelDtos)
            {
                loadModelQueue.Enqueue(loadModelDto);
            }
            
            if (!isProcessingLoadModelQueue)
            {
                StartCoroutine(ProcessLoadModelQueue());
            }
        }
    }
}