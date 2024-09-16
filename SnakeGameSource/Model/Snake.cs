using System.Collections;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Common;
using SnakeGameSource.GameEngine.Common.Components;
using SnakeGameSource.GameEngine.Extensions;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource.Model;

internal sealed class Snake : ISnake
{
    private readonly IGrid _grid;

    private readonly List<GameObject> _projectedSnakeParts = [];
    private readonly List<GameObject> _snakeParts          = [];

    private float _maxSpeed;
    private float _minSpeed;
    
    private Vector2 _scale = Vector2.One;

    public Snake(SnakeConfig config, IGrid grid)
    {
        MoveSpeed = config.MoveSpeed;
        SlewingSpeed = config.SlewingSpeed;
        Direction = config.StartDirection;
        _minSpeed = config.MinSpeed;
        _maxSpeed = config.MaxSpeed;
        _grid = grid;
        
        CreateHead(config);
        CreateBody(config);

        UpdateProjectedSnakeParts();
        ProjectedHead.AddComponent<CollisionNotifier>().CollisionEnter += OnCollisionEnter;
    }

    //TODO: Каждые 10 очков смена фона и сделать маленькую надпись скора в углу
    public int Score { get; private set; }

    public IEnumerator<GameObject> GetEnumerator() => _projectedSnakeParts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Vector2 Position => Head.Transform.Position;

    public float MoveSpeed { get; private set; }

    public Vector2 Direction { get; }

    public Vector2 Scale
    {
        get => _scale;

        private set
        {
            _scale = value;
            foreach (GameObject gameObject in _snakeParts.AsSpan())
            {
                gameObject.Transform.Scale = _scale;
            }
        }
    }

    public float SlewingSpeed { get; }
    
    private GameObject Head => _snakeParts[0];

    private GameObject ProjectedHead => _projectedSnakeParts[0];

    public void MoveTo(Vector2 nextPosition)
    {
        if (nextPosition == Position) return;
        using SpanOwner<Vector2> offsets = CalculateOffsets(nextPosition);
        ApplyOffsets(offsets.Span);
        UpdateProjectedSnakeParts();
    }

    public event Action? Die;

    private void CreateHead(SnakeConfig config) =>
        _snakeParts.Add(new GameObject("Snake Head")
            .WithTransform(config.StartPosition, Scale)
            .WithTextureConfig(TextureName.SnakeHead, config.HeadColor)
            .WithComponent(config.ColliderType));

    private void CreateBody(SnakeConfig config)
    {
        _snakeParts.Add(new GameObject()
            .WithTransform(config.StartPosition - Direction * Scale, Scale)
            .WithTextureConfig(TextureName.SnakeBody, config.BodyColor));

        for (var i = 1; i < config.InitialLength; i++)
        {
            _snakeParts.Add(new GameObject()
                .WithTransform(config.StartPosition - Direction * Scale * (i + 1), Scale)
                .WithComponent(config.ColliderType)
                .WithTextureConfig(TextureName.SnakeBody, config.BodyColor));
        }
    }

    private SpanOwner<Vector2> CalculateOffsets(Vector2 nextPosition)
    {
        SpanOwner<Vector2> spanOwner = SpanOwner<Vector2>.Allocate(_snakeParts.Count);
        Span<Vector2>      offsets   = spanOwner.Span;

        offsets[0] = nextPosition - Position;
        ReadOnlySpan<GameObject> span = _snakeParts.AsSpan();

        for (var i = 1; i < span.Length; i++)
        {
            Transform transform1 = span[i].Transform;
            Transform transform2 = span[i - 1].Transform;

            offsets[i] =  transform2.Position - transform1.Position;
            offsets[i] /= Scale;
        }

        return spanOwner;
    }

    private void ApplyOffsets(Span<Vector2> offsets)
    {
        if (offsets.Length != _snakeParts.Count)
        {
            throw new ArgumentException("Count of offsets and snake parts don't match");
        }
        Head.Transform.Position += offsets[0];
        float offsetLength = offsets[0].Length();

        var counter = 1;
        foreach (GameObject snakePart in _snakeParts.AsSpan()[1..])
        {
            snakePart.Transform.Position += offsets[counter++] * offsetLength;
        }
    }

    private void UpdateProjectedSnakeParts()
    {
        if (_snakeParts.Count > _projectedSnakeParts.Count)
        {
            for (int i = _projectedSnakeParts.Count; i < _snakeParts.Count; i++)
            {
                _projectedSnakeParts.Add(_snakeParts[i].Clone());
            }
        }
        else if (_snakeParts.Count < _projectedSnakeParts.Count)
        {
            for (int i = _snakeParts.Count; i < _projectedSnakeParts.Count; i++)
            {
                _projectedSnakeParts.RemoveAt(i);
            }
        }

        for (var i = 0; i < _projectedSnakeParts.Count; i++)
        {
            UpdateProjectedTransform(i);
        }
    }

    private void UpdateProjectedTransform(int snakePartIndex)
    {
        Transform transform1 = _snakeParts[snakePartIndex].Transform;
        Transform transform2 = _projectedSnakeParts[snakePartIndex].Transform;

        transform1.TryCopyTo(transform2);
        transform2.Position = _grid.Project(transform2.Position);
    }

    private void OnCollisionEnter(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out FoodEffect? effect))
        {
            ApplyFoodEffect(effect);
            Score++;
        }

        //Die?.Invoke();
    }

    private void ApplyFoodEffect(FoodEffect effect)
    {
        switch (effect.Type)
        {
            case FoodEffectType.Speed:
                if (IsValidSpeed(MoveSpeed + effect.Value)) MoveSpeed += effect.Value;
                break;

            case FoodEffectType.Scale:
                if (Scale.X + effect.Value > 0.5f && Scale.Y + effect.Value > 0.5f) Scale += new Vector2(effect.Value);
                break;

            case FoodEffectType.Length:
                if (effect.Value > 0) AddSnakePart();
                else if (_snakeParts.Count - 1 > 2) RemoveSnakePart(_snakeParts.Count - 1);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(effect), $"No such effect type {effect.Type}");
        }
    }

    private bool IsValidSpeed(float speed) => speed > _minSpeed && speed < _maxSpeed;

    private void RemoveSnakePart(int snakePartIndex) => _snakeParts.RemoveAt(snakePartIndex);

    private void AddSnakePart() => _snakeParts.Add(_snakeParts[^1].Clone());
}