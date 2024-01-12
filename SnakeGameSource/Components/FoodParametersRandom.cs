using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.Components;

public class FoodParametersRandom : Component
{
    private readonly Effect   _commonEffect = new();
    private readonly Effect[] _effects      = new Effect[5];
    private readonly Random   _random       = new();

    private TimeSpan _remainLifetime;

    public Grid? Grid { get; set; }

    public int FoodLifetime { get; set; }

    private void Awake()
    {
        for (var i = 0; i < _effects.Length; i++)
        {
            _effects[i] = new Effect();
        }

        _commonEffect.Type  = EffectType.Length;
        _commonEffect.Value = 1;

        _effects[0].Type   = EffectType.Speed;
        _effects[0].Value  = 0.4f;
        _effects[0].Chance = 10;

        _effects[1].Type   = EffectType.Speed;
        _effects[1].Value  = 0.4f;
        _effects[1].Chance = 10;

        _effects[2].Type   = EffectType.Scale;
        _effects[2].Value  = 0.05f;
        _effects[3].Chance = 10;

        _effects[3].Type   = EffectType.Scale;
        _effects[3].Value  = -0.05f;
        _effects[3].Chance = 10;

        _effects[4].Type   = EffectType.Length;
        _effects[4].Value  = -1;
        _effects[4].Chance = 1;
    }

    private void Update(TimeSpan delta)
    {
        _remainLifetime -= delta;

        if (_remainLifetime.TotalSeconds <= 0)
        {
            RandFoodParameters();
        }
    }

    private void OnCollisionEnter(GameObject gameObject)
    {
        if (gameObject.Name is "Snake head")
        {
            RandFoodParameters();
        }
    }

    private void RandFoodParameters()
    {
        RandEffect();
        RandPosition();

        _remainLifetime = TimeSpan.FromSeconds(FoodLifetime);
    }

    private void RandEffect()
    {
        int effectIndex = _random.Next(0, _effects.Length);
        int number      = _random.Next(0, 101);
        SetEffect(number <= _effects[effectIndex].Chance * _effects.Length
                      ? _effects[effectIndex]
                      : _commonEffect);
    }

    private void SetEffect(Effect effect)
    {
        effect.TryCopyTo(GetComponent<Effect>());
    }

    private void RandPosition()
    {
        if (Grid is null)
        {
            throw new NullReferenceException(nameof(Grid) + "must be not null");
        }

        var transform = GetComponent<Transform>();
        do
        {
            transform.Position = new Vector2(_random.Next(1, Grid.Size.X - 1),
                                             _random.Next(1, Grid.Size.Y - 1));
        } while (Grid.IsPositionOccupied(transform.Position, transform.Scale));
    }
}