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

        private Vector2[] _directions;
        private float _scale = 1f;
        private int _lastColliderIndex;

        public Snake(SnakeConfig snakeConfig, Grid grid)
        {
            MoveSpeed = snakeConfig.MoveSpeed;
            SlewingTime = snakeConfig.SlewingTime;
            _lastColliderIndex = snakeConfig.StartBodyLength;
            _bodyColor = snakeConfig.BodyColor;
            _grid = grid;
            _directions = new Vector2[snakeConfig.StartBodyLength + 1];

            for (int i = 0; i < _directions.Length; i++)
            {
                _directions[i] = snakeConfig.StartDirection;
            }

            for (int i = 0; i <= snakeConfig.StartBodyLength; i++)
            {
                GameObject bodyPart = i switch
                {
                    0 => new("Snake head"),
                    _ => new()
                };

                Transform bodyPartTransform = bodyPart.AddComponent<Transform>();
                bodyPartTransform.Position = snakeConfig.StartPosition - (Direction * Scale * i);
                bodyPartTransform.Scale = Scale;

                TextureConfig bodyPartTexture = bodyPart.AddComponent<TextureConfig>();
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

            GameObject headClone = Head.Clone();
            headClone.GetComponent<Collider>().CollisionEnter += OnCollisionEnter;
            _projectedBody.Add(headClone);

            UpdateProjectedBody();
        }

        public Vector2 Position => Head.GetComponent<Transform>().Position;

        public float MoveSpeed { get; private set; }

        public float SlewingTime { get; private set; }

        public Vector2 Direction
        {
            get { return _directions[0]; }
            private set { _directions[0] = value; }
        }

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

            Vector2[] offsets = CalculateOffsets(nextPosition);
            Vector2[] nextDirections = CalculateDirections(offsets);
            float[] rotations = CalculateRotations(nextDirections);

            ApplyOffsets(offsets);
            ApplyDirections(nextDirections);
            ApplyRotations(rotations);

            CheckColliders();
            UpdateProjectedBody();
        }

        private Vector2[] CalculateOffsets(Vector2 nextPosition)
        {
            Vector2[] offsets = new Vector2[_body.Count];

            offsets[0] = nextPosition - Position;

            for (int i = 1; i < _body.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _body[i - 1].GetComponent<Transform>();

                offsets[i] = transform2.Position - transform1.Position;
                offsets[i] /= Scale;
            }

            return offsets;
        }

        private Vector2[] CalculateDirections(Vector2[] offsets)
        {
            Vector2[] directions = new Vector2[offsets.Length];

            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = offsets[i] != Vector2.Zero ? Vector2.Normalize(offsets[i]) : Vector2.Zero;
            }

            return directions;
        }

        private float[] CalculateRotations(Vector2[] nextDirections)
        {
            float[] rotations = new float[nextDirections.Length];

            for (int i = 0; i < rotations.Length; i++)
            {
                if (_directions.Length <= i)
                {
                    rotations[i] = 0;
                }
                else
                {
                    float cosa = Vector2.Dot(_directions[i], nextDirections[i]);
                    Vector2 vector = _directions[i] - nextDirections[i];
                    bool isNegative;
                    rotations[i] = MathF.Asin(cosa);
                }
            }

            return rotations;
        }

        private void ApplyOffsets(Vector2[] offsets)
        {
            Head.GetComponent<Transform>().Position += offsets[0];

            for (int i = 1; i < offsets.Length; i++)
            {
                _body[i].GetComponent<Transform>().Position += offsets[i] * offsets[0].Length();
            }
        }

        private void ApplyDirections(Vector2[] nextDirections)
        {
            _directions = nextDirections;
        }

        private void ApplyRotations(float[] rotations)
        {
            for (int i = 0; i < _body.Count; i++)
            {
                Transform transform = _body[i].GetComponent<Transform>();
                Quaternion rotation = transform.Rotation;
                transform.Rotation = rotation;
            }
        }

        private void UpdateProjectedBody()
        {
            if (_body.Count > _projectedBody.Count)
            {
                for (int i = _projectedBody.Count; i < _body.Count; i++)
                {
                    _projectedBody.Add(_body[i].Clone());
                }
            }
            else if (_body.Count < _projectedBody.Count)
            {
                for (int i = _body.Count; i < _projectedBody.Count; i++)
                {
                    _projectedBody.RemoveAt(i);
                }
            }

            for (int i = 0; i < _projectedBody.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _projectedBody[i].GetComponent<Transform>();

                transform1.CopyTo(transform2);
                transform2.Position = _grid.Project(transform2.Position);

                TryCloneCollider(_projectedBody[i], _body[i]);
            }
        }

        private void CheckColliders()
        {
            for (int i = _lastColliderIndex + 1; i < _body.Count; i++)
            {
                Transform transform1 = _body[i].GetComponent<Transform>();
                Transform transform2 = _body[i - 1].GetComponent<Transform>();
                Collider collider = _body[i - 1].GetComponent<Collider>();

                if (_body[i].TryGetComponent<Collider>() is null
                    && Vector2.Distance(transform1.Position, transform2.Position) <= 0.6f * Scale)
                {
                    _body[i].AddComponent(collider.GetType());
                    _lastColliderIndex = i;
                }
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

        private void TryCloneCollider(GameObject dest, GameObject source)
        {
            Collider? collider1 = source.TryGetComponent<Collider>();
            Collider? collider2 = dest.TryGetComponent<Collider>();

            if (collider2 is null && collider1 is not null)
            {
                dest.AddComponent(collider1.GetType());
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