using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.Components;

public enum FoodEffectType
{
    Speed,
    Scale,
    Length
}

public sealed class FoodEffect : Component
{
    public float Value { get; set; }

    public FoodEffectType Type { get; set; }

    public float Chance { get; set; }

    public override bool TryCopyTo(Component component)
    {
        if (component is not FoodEffect effect)
        {
            return false;
        }

        effect.Value  = Value;
        effect.Type   = Type;
        effect.Chance = Chance;

        return true;
    }
}