using UnityEngine;

namespace Prism.Web.Dto
{
    [System.Serializable]
    public class Vector3Dto
    {
        public float x;
        public float y;
        public float z;

        public static Vector3Dto FromVector3(Vector3 vector)
        {
            return new Vector3Dto
            {
                x = vector.x,
                y = vector.y,
                z = vector.z,
            };
        }

        public static Vector3 ToVector3(Vector3Dto dto)
        {
            return new Vector3(dto.x, dto.y, dto.z);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
}