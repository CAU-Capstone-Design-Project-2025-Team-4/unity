namespace Prism.Web.Dto
{
    [System.Serializable]
    public class AnimationControlDto
    {
        public string id;
        public string action;
        public string animationName;
        public float speed = 1.0f;
        public float normalizedTime;
    }
}