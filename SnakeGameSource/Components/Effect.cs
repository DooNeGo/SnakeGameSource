using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.Components;

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

    public float Chance { get; set; }

    public override bool TryCopyTo<T>(T component)
    {
        if (component is not Effect effect)
        {
            return false;
        }
        
        effect.Value  = Value;
        effect.Type   = Type;
        effect.Chance = Chance;

        return true;
    }
}