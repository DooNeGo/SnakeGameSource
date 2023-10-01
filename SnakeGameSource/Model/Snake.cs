using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.Components.Colliders;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SnakeGameSource.Model
{
    internal class Snake : IMovable, IEnumerable<GameObject>
    {
        public event Action? Die;

        private readonly List<GameObject> _body = new();
        private readonly List<GameObject> _projectedBody = new();
        private readonly Grid _grid;
        private readonly Color _bodyColor;

        private float _scale = 1f;
        private int _lastColliderIndex;

        public Snake(SnakeConfig snakeConfig, Grid grid)
        {
            MoveSpeed = snakeConfig.MoveSpeed;
            SlewingTime = snakeConfig.SlewingTime;
            Direction = snakeConfig.StartDirection;
            _lastColliderIndex = snakeConfig.StartBodyLength;
            _bodyColor = snakeConfig.BodyColor;
            _grid = grid;

            for (var i = 0; i <= snakeConfig.StartBodyLength; i++)
            {
                GameObject bodyPart = i switch
                {
                    0 => new("Snake head"),
                    _ => new()
                };
                Transform bodyPartTransform = bodyPart.AddComponent<Transform>();
                TextureConfig bodyPartTexture = bodyPart.AddComponent<TextureConfig>();
                bodyPartTransform.Scale = Scale;
                bodyPartTransform.Position = snakeConfig.StartPosition - Direction * Scale * i;

                if (i == 0)
                {
                    bodyPartTexture.Color = snakeConfig.HeadColor;
                    bodyPartTexture.Name = TextureName.SnakeHead;
                }
                else
                {
                    bodyPartTexture.Color = _bodyColor;
                    bodyPartTexture.Name = TextureName.SnakeBody;
                }

                if (i != 1)
                {
                    bodyPart.AddComponent<CircleCollider>();
                }

                _body.Add(bodyPart);
            }

            UpdateProjectedBody();
        }

        public Vector2 Position => Head.GetComponent<Transform>().Position;

        public float MoveSpeed { get; private set; }

        public float SlewingTime { get; private set; }

        public Vector2 Direction { get; private set; }

        public int Score { get; private set; } = 0;

        public float Scale
        {
            get { return _scale; }
            private set
            {
                _scale = value;
                foreach (GameObject body in _body)
                {
                    body.GetComponent<Transform>().Scale = _scale;
                }
            }
        }

        private GameObject Head => _body[0];

        public void MoveTo(Vector2 nextPosition)
        {
            if (nextPosition == Position)
                return;

            Direction = Vector2.Normalize(nextPosition - Position);

            ApplyOffsets(CalculateOffsets(nextPosition));
            CheckColliders();
            UpdateProjectedBody();
        }

        private void UpdateProjectedBody()
        {
            if (_body.Count > _projectedBody.Count)
            {
                for (var i = _projectedBody.Count; i < _body.Count; i++)
                {
                    _projectedBody.Add(CloneWithProjection(_body[i]));
                }
            }
            else if (_body.Count < _projectedBody.Count)
            {
                for (var i = _body.Count; i < _projectedBody.Count; i++)
                {
                    _projectedBody.RemoveAt(i);
                }
            }

            for (int i = 0; i < _projectedBody.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _projectedBody[i].GetComponent<Transform>();

                TryCloneCollider(_projectedBody[i], _body[i]);
                transform1.CopyTo(transform2);
                transform2.Position = _grid.Project(transform2.Position);
            }
        }

        private void CheckColliders()
        {
            for (int i = _lastColliderIndex + 1; i < _body.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _body[i - 1].GetComponent<Transform>();

                if (_body[i].TryGetComponent<Collider>() is null
                    && Vector2.Distance(transform1.Position, transform2.Position) <= 0.6f * Scale)
                {
                    _body[i].AddComponent<CircleCollider>();
                    _lastColliderIndex = i;
                }
            }
        }

        private Vector2[] CalculateOffsets(Vector2 nextPosition)
        {
            var offsets = new Vector2[_body.Count];

            offsets[0] = nextPosition - Position;

            for (var i = 1; i < _body.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _body[i - 1].GetComponent<Transform>();

                offsets[i] = transform2.Position - transform1.Position;
                offsets[i] /= Scale;
            }

            return offsets;
        }

        private void ApplyOffsets(Vector2[] offsets)
        {
            Head.GetComponent<Transform>().Position += offsets[0];

            for (var i = 1; i < offsets.Length; i++)
            {
                _body[i].GetComponent<Transform>().Position += offsets[i] * offsets[0].Length();
            }
        }

        private void OnCollisionEnter(GameObject gameObject)
        {
            if (gameObject is Food food)
                Eat(food);
            else
                Die?.Invoke();
        }

        private void Eat(Food food)
        {
            float effectValue = food.GetComponent<Effect>().Value;

            switch (food.GetComponent<Effect>().Type)
            {
                case EffectType.Speed:
                    if (MoveSpeed + effectValue > 2)
                        MoveSpeed += effectValue;
                    break;

                case EffectType.Scale:
                    if (Scale + effectValue > 0.5f)
                        Scale += effectValue;
                    break;

                case EffectType.Length:
                    if (effectValue > 0)
                        AddNewBodyPart();
                    else if (_body.Count - 1 > 2)
                        _body.RemoveAt(_body.Count - 1);
                    break;
            }

            Score++;
        }

        private void AddNewBodyPart()
        {
            Transform tailTransform = _body[^1].GetComponent<Transform>();
            TextureConfig tailTexture = _body[^1].GetComponent<TextureConfig>();

            Vector2 tailProjection = _grid.Project(tailTransform.Position);
            Vector2 offset = new(tailTransform.Position.X - tailProjection.X, tailTransform.Position.Y - tailProjection.Y);
            Vector2 projectionOnTheEdge = _grid.GetTheClosestProjectionOnTheEdge(tailProjection);

            GameObject newBodyPart = new();

            Transform newBodyPartTransform = newBodyPart.AddComponent<Transform>();
            tailTransform.CopyTo(newBodyPartTransform);
            newBodyPartTransform.Position = projectionOnTheEdge + offset;

            TextureConfig newBodyPartTexture = newBodyPart.AddComponent<TextureConfig>();
            tailTexture.CopyTo(newBodyPartTexture);

            _body.Add(newBodyPart);
        }

        private GameObject CloneWithProjection(GameObject gameObject)
        {
            Transform bodyTransform = gameObject.GetComponent<Transform>();
            TextureConfig bodyTexture = gameObject.GetComponent<TextureConfig>();

            GameObject clone = gameObject.Name switch
            {
                null => new(),
                _ => new(gameObject.Name)
            };

            Transform transform = clone.AddComponent<Transform>();
            bodyTransform.CopyTo(transform);
            transform.Position = _grid.Project(transform.Position);

            TryCloneCollider(clone, gameObject);

            TextureConfig texture = clone.AddComponent<TextureConfig>();
            bodyTexture.CopyTo(texture);

            return clone;
        }

        private void TryCloneCollider(GameObject dest, GameObject source)
        {
            Collider? collider1 = source.TryGetComponent<Collider>();
            Collider? collider2 = dest.TryGetComponent<Collider>();

            if (collider2 is null && collider1 is not null)
            {
                collider2 = collider1 switch
                {
                    SquareCollider => dest.AddComponent<SquareCollider>(),
                    CircleCollider => dest.AddComponent<CircleCollider>(),
                    _ => throw new NotImplementedException()
                };

                if (source == Head)
                    collider2.CollisionEntry += OnCollisionEnter;
            }
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return _projectedBody.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _projectedBody.GetEnumerator();
        }
    }
}