namespace SnakeGameSource.Components
{
    public enum EffectType
    {
        Speed,
        Scale,
        Length
    }

    internal class Effect : Component
    {
        public float Value { get; set; }

        public EffectType Type { get; set; }
    }
}