namespace Prism.Web.Dto
{
    [System.Serializable]
    public class AnimationStateDto
    {
        public string id;
        public string currentAnimation;
        public bool isPlaying;
        public float normalizedTime;
        public string[] availableAnimations;
        public string error;
    }
}