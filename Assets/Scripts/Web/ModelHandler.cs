using UnityEngine;
using Prism.Web.Dto;

namespace Prism.Web
{
    public class ModelHandler : MonoBehaviour
    {
        [SerializeField] private bool useInitialSettings;
        [SerializeField] private string initialId;
        [SerializeField] private Vector3 initialPosition;
        [SerializeField] private Vector3 initialRotation;
        [SerializeField] private Vector3 initialScale;
        [SerializeField] private string initialShader;
        
        public void SetModelProperties(string jsonString)
        {
            var data = JsonUtility.FromJson<ModelPropertiesDto>(jsonString);
            var id = data.id;
            
            if (!ModelManager.Instance.TryGetModel(id, out var model)) return;

            var position = new Vector3(data.transform.position.x, data.transform.position.y, data.transform.position.z);
            var rotation = Quaternion.Euler(data.transform.rotation.x, data.transform.rotation.y, data.transform.rotation.z);
            var scale = new Vector3(data.transform.scale.x, data.transform.scale.y, data.transform.scale.z);
            var shader = data.shader;
            
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

            var initialModelPropertiesDto = new ModelPropertiesDto
            {
                id = initialId,
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
            };
            
            var jsonString = JsonUtility.ToJson(initialModelPropertiesDto);
            
            JsonUtility.FromJson<ModelPropertiesDto>(jsonString);
        }
    }
}