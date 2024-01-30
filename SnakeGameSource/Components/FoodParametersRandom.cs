using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Components;

public class FoodParametersRandom : Component
{
    private readonly Effect   _commonEffect = new();
    private readonly Effect[] _effects      = new Effect[5];
    private readonly Random   _random       = new();

    private int _lastEffectIndex;

    public IGrid? Grid { get; set; }

    public ICollisionHandler? CollisionHandler { get; set; }

    public TimeSpan FoodLifetime { get; set; }

    public TimeSpan RemainFoodLifetime { get; private set; }

    private void Awake()
    {
        for (var i = 0; i < _effects.Length; i++)
        {
            _effects[i] = new Effect();
        }

        _commonEffect.Type  = EffectType.Length;
        _commonEffect.Value = 1;

        _effects[0].Type   = EffectType.Speed;
        _effects[0].Value  = 0.3f;
        _effects[0].Chance = 10;

        _effects[1].Type   = EffectType.Speed;
        _effects[1].Value  = -0.3f;
        _effects[1].Chance = 10;

        _effects[2].Type   = EffectType.Scale;
        _effects[2].Value  = 0.08f;
        _effects[2].Chance = 10;

        _effects[3].Type   = EffectType.Scale;
        _effects[3].Value  = -0.08f;
        _effects[3].Chance = 10;

        _effects[4].Type   = EffectType.Length;
        _effects[4].Value  = -1;
        _effects[4].Chance = 5;
    }

    private void Update(TimeSpan delta)
    {
        RemainFoodLifetime -= delta;

        if (RemainFoodLifetime.TotalSeconds <= 0)
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

        RemainFoodLifetime = FoodLifetime;
    }

    private void RandEffect()
    {
        int effectChance = _random.Next(0, 101);
        SetEffect(effectChance <= _effects[_lastEffectIndex].Chance ? _effects[_lastEffectIndex] : _commonEffect);
        if (_lastEffectIndex + 1 == _effects.Length - 1)
        {
            _lastEffectIndex = 0;
        }
        else
        {
            _lastEffectIndex++;
        }
    }

    private void SetEffect(Effect effect)
    {
        effect.TryCopyTo(GetComponent<Effect>() 
            ?? throw new NullReferenceException("There is no 'Effect' component"));
    }

    private void RandPosition()
    {
        if (Grid is null)
        {
            throw new NullReferenceException(nameof(Grid) + "must be not null");
        }

        if (CollisionHandler is null)
        {
            throw new NullReferenceException(nameof(CollisionHandler) + "must be not null");
        }

        Transform transform    = Parent!.Transform;
        Vector2   scale        = transform.Scale;
        Type      colliderType = typeof(SquareCollider);

        if (TryGetComponent(out Collider? collider))
        {
            scale *= collider.Scale;
            colliderType = collider.GetType();
        }

        do
        {
            transform.Position = new Vector2(_random.Next(1, Grid.Size.X - 1), _random.Next(1, Grid.Size.Y - 1));
        }
        while (CollisionHandler.IsCollidingWithAnyCollider(colliderType, transform.Position, scale));
    }

    public override bool TryCopyTo(Component component)
    {
        if (component is not FoodParametersRandom random)
        {
            return false;
        }

        random.Grid               = Grid;
        random.RemainFoodLifetime = RemainFoodLifetime;
        random.FoodLifetime       = FoodLifetime;

        return true;
    }
}