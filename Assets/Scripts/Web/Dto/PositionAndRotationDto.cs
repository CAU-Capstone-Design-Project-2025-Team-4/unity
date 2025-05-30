using UnityEngine;

namespace Prism.Web.Dto
{
    [System.Serializable]
    public class PositionAndRotationDto
    {
        public Vector3Dto position;
        public Vector3Dto rotation;

        public static PositionAndRotationDto FromPositionAndRotation(Vector3 position, Vector3 rotation)
        {
            return new PositionAndRotationDto
            {
                position = Vector3Dto.FromVector3(position),
                rotation = Vector3Dto.FromVector3(rotation),
            };
        }
    }
}