namespace Assets.Scripts.Perlin
{
    [System.Serializable]
    public struct NoiseConfig
    {
        public float frequency;
        public float magnitude;

        public override string ToString()
        {
            return string.Format("freq{0}, mag{1}", frequency, magnitude);
        }
    }
}
