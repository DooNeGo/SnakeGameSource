using CommunityToolkit.Diagnostics;
using Microsoft.Xna.Framework;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.GameEngine.Components.Colliders;

namespace SnakeGameSource.Components;

public sealed class FoodParametersRandom : Component
{
    private readonly FoodEffect   _commonEffect = new();
    private readonly FoodEffect[] _effects      = new FoodEffect[5];
    private readonly Random       _random       = new();

    private int _lastEffectIndex;

    public IGrid? Grid { get; set; }

    public ICollisionHandler? CollisionHandler { get; set; }

    public TimeSpan FoodLifetime { get; set; }

    public TimeSpan RemainFoodLifetime { get; private set; }

    protected override void Awake()
    {
        for (var i = 0; i < _effects.Length; i++)
        {
            _effects[i] = new FoodEffect();
        }

        _commonEffect.Type  = FoodEffectType.Length;
        _commonEffect.Value = 1;

        _effects[0].Type   = FoodEffectType.Speed;
        _effects[0].Value  = 0.3f;
        _effects[0].Chance = 10;

        _effects[1].Type   = FoodEffectType.Speed;
        _effects[1].Value  = -0.3f;
        _effects[1].Chance = 10;

        _effects[2].Type   = FoodEffectType.Scale;
        _effects[2].Value  = 0.08f;
        _effects[2].Chance = 10;

        _effects[3].Type   = FoodEffectType.Scale;
        _effects[3].Value  = -0.08f;
        _effects[3].Chance = 10;

        _effects[4].Type   = FoodEffectType.Length;
        _effects[4].Value  = -1;
        _effects[4].Chance = 5;
    }

    protected override void Update(TimeSpan time)
    {
        RemainFoodLifetime -= time;

        if (RemainFoodLifetime.TotalSeconds <= 0)
        {
            RandFoodParameters();
        }
    }

    protected override void OnCollisionEnter(GameObject gameObject)
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

    private void SetEffect(FoodEffect effect)
    {
        effect.TryCopyTo(GetRequiredComponent<FoodEffect>());
    }

    private void RandPosition()
    {
        Guard.IsNotNull(Grid);
        Guard.IsNotNull(CollisionHandler);

        Transform transform    = Parent!.Transform;
        Vector2   scale        = transform.Scale;
        Type      colliderType = typeof(SquareCollider);

        if (TryGetComponent(out Collider? collider))
        {
            scale        *= collider.Scale;
            colliderType =  collider.GetType();
        }

        Vector2 nextPosition;
        do
        {
            nextPosition = new Vector2(_random.Next(1, Grid.Size.X - 1), _random.Next(1, Grid.Size.Y - 1));
        }
        while (CollisionHandler.IsCollidingWithAnyCollider(colliderType, nextPosition, scale));

        transform.Position = nextPosition;
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