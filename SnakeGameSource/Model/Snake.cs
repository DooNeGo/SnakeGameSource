using System.Collections;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Buffers;
using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Abstractions;
using SnakeGameSource.GameEngine.Components;
using SnakeGameSource.Model.Abstractions;

namespace SnakeGameSource.Model;

internal sealed class Snake : ISnake
{
    private readonly IGrid _grid;

    private readonly List<GameObject> _projectedSnakeParts = [];
    private readonly List<GameObject> _snakeParts          = [];

    private Vector2[] _directions;

    private float   _maxSpeed;
    private float   _minSpeed;
    private Vector2 _scale = new(1f);

    public Snake(SnakeConfig snakeConfig, IGrid grid)
    {
        MoveSpeed    = snakeConfig.MoveSpeed;
        SlewingSpeed = snakeConfig.SlewingSpeed;
        Direction    = snakeConfig.StartDirection;
        _grid        = grid;
        _directions  = new Vector2[snakeConfig.InitialLength + 1];

        for (var i = 0; i < _directions.Length; i++)
        {
            _directions[i] = snakeConfig.StartDirection;
        }

        for (var i = 0; i <= snakeConfig.InitialLength; i++)
        {
            GameObject snakePart = i is 0 ? new GameObject("Snake head") : new GameObject();

            Transform transform = snakePart.Transform;
            transform.Position = snakeConfig.StartPosition - Direction * Scale * i;
            transform.Scale    = Scale;

            var texture = snakePart.AddComponent<TextureConfig>();
            if (i is 0)
            {
                texture.Color = snakeConfig.HeadColor;
                texture.Name  = TextureName.SnakeHead;
            }
            else
            {
                texture.Color = snakeConfig.BodyColor;
                texture.Name  = TextureName.SnakeBody;
            }

            if (i is not 1)
            {
                snakePart.AddComponent(snakeConfig.ColliderType);
            }

            _snakeParts.Add(snakePart);
        }

        UpdateProjectedSnakeParts();
        ProjectedHead.AddComponent<CollisionNotifier>().CollisionEnter += OnCollisionEnter;
    }

    private GameObject Head => _snakeParts[0];

    private GameObject ProjectedHead => _projectedSnakeParts[0];

    //TODO: Каждые 10 очков смена фона и сделать маленькую надпись скора в углу
    public int Score { get; private set; }

    public IEnumerator<GameObject> GetEnumerator()
    {
        return _projectedSnakeParts.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _projectedSnakeParts.GetEnumerator();
    }

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

    public void MoveTo(Vector2 nextPosition)
    {
        if (nextPosition == Position)
        {
            return;
        }

        using SpanOwner<Vector2> offsets = CalculateOffsets(nextPosition);

        //Vector2[] nextDirections = CalculateDirections(offsets);
        //float[] rotations = CalculateRotations(nextDirections);

        ApplyOffsets(offsets.Span);

        //ApplyDirections(nextDirections);
        //ApplyRotations(rotations);

        UpdateProjectedSnakeParts();
    }

    public event Action? Die;

    private SpanOwner<Vector2> CalculateOffsets(Vector2 nextPosition)
    {
        SpanOwner<Vector2> spanOwner = SpanOwner<Vector2>.Allocate(_snakeParts.Count);
        Span<Vector2>      offsets   = spanOwner.Span;

        offsets[0] = nextPosition - Position;
        Span<GameObject> span = _snakeParts.AsSpan();

        for (var i = 1; i < span.Length; i++)
        {
            Transform transform1 = span[i].Transform;
            Transform transform2 = span[i - 1].Transform;

            offsets[i] =  transform2.Position - transform1.Position;
            offsets[i] /= Scale;
        }

        return spanOwner;
    }

    private Vector2[] CalculateDirections(Vector2[] offsets)
    {
        var directions = new Vector2[offsets.Length];

        for (var i = 0; i < directions.Length; i++)
        {
            directions[i] = offsets[i] != Vector2.Zero ? Vector2.Normalize(offsets[i]) : Vector2.Zero;
        }

        return directions;
    }

    private float[] CalculateRotations(Vector2[] nextDirections)
    {
        var rotations = new float[nextDirections.Length];

        for (var i = 0; i < rotations.Length; i++)
        {
            if (_directions.Length <= i)
            {
                rotations[i] = 0;
            }
            else
            {
                float   cos    = Vector2.Dot(_directions[i], nextDirections[i]);
                Vector2 vector = _directions[i] - nextDirections[i];
                rotations[i] = MathF.Asin(cos);
            }
        }

        return rotations;
    }

    private void ApplyOffsets(Span<Vector2> offsets)
    {
        Head.Transform.Position += offsets[0];

        for (var i = 1; i < offsets.Length; i++)
        {
            _snakeParts[i].Transform.Position += offsets[i] * offsets[0].Length();
        }
    }

    private void ApplyDirections(Vector2[] nextDirections)
    {
        _directions = nextDirections;
    }

    private void ApplyRotations(float[] rotations)
    {
        foreach (GameObject gameObject in _snakeParts)
        {
            Transform  transform = gameObject.Transform;
            Quaternion rotation  = transform.Rotation;
            transform.Rotation = rotation;
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
                if (MoveSpeed + effect.Value > 2)
                {
                    MoveSpeed += effect.Value;
                }

                break;

            case FoodEffectType.Scale:
                if (Scale.X + effect.Value > 0.5f && Scale.Y + effect.Value > 0.5f)
                {
                    Scale += new Vector2(effect.Value);
                }

                break;

            case FoodEffectType.Length:
                if (effect.Value > 0)
                {
                    AddSnakePart();
                }
                else if (_snakeParts.Count - 1 > 2)
                {
                    RemoveSnakePart(_snakeParts.Count - 1);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(effect), $"No such effect type {effect.Type}");
        }
    }

    private void RemoveSnakePart(int snakePartIndex)
    {
        _snakeParts.RemoveAt(snakePartIndex);
    }

    private void AddSnakePart()
    {
        _snakeParts.Add(_snakeParts[^1].Clone());
    }
}