using Microsoft.Xna.Framework;
using SnakeGameSource.Components;
using SnakeGameSource.Components.Colliders;
using SnakeGameSource.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SnakeGameSource.Controllers
{
    internal class FoodController : IEnumerable<GameObject>
    {
        private const float foodScale = 0.5f;

        private readonly Food[] _foods;
        private readonly Random _random = new();
        private readonly Grid _grid;

        private Index _activeFoodIndex = 0;
        private TimeSpan _remainTime;

        public FoodController(Grid grid)
        {
            _grid = grid;

            _foods = new Food[]
            {
                new Food(_grid.Center, foodScale, Color.Red, 15),
                new Food(_grid.Center, foodScale, Color.Blue, 10),
                new Food(_grid.Center, foodScale, Color.Blue, 10),
                new Food(_grid.Center, foodScale, Color.Lime, 10),
                new Food(_grid.Center, foodScale, Color.Lime, 10),
                new Food(_grid.Center, foodScale, Color.White, 5),
            };

            Effect effect0 = _foods[0].AddComponent<Effect>();
            Effect effect1 = _foods[1].AddComponent<Effect>();
            Effect effect2 = _foods[2].AddComponent<Effect>();
            Effect effect3 = _foods[3].AddComponent<Effect>();
            Effect effect4 = _foods[4].AddComponent<Effect>();
            Effect effect5 = _foods[5].AddComponent<Effect>();

            effect0.Type = EffectType.Length;
            effect0.Value = 1;
            effect1.Type = EffectType.Speed;
            effect1.Value = 0.5f;
            effect2.Type = EffectType.Speed;
            effect2.Value = 0.5f;
            effect3.Type = EffectType.Scale;
            effect3.Value = 0.1f;
            effect4.Type = EffectType.Scale;
            effect4.Value = -0.1f;
            effect5.Type = EffectType.Length;
            effect5.Value = -1;

            for (int i = 0; i < _foods.Length; i++)
            {
                _foods[i].GetComponent<Collider>().CollisionEnter += OnCollisionEnter;
            }

            _remainTime = TimeSpan.FromSeconds(ActiveFood.LifeTime);
        }

        private Food ActiveFood => _foods[_activeFoodIndex];

        public void Update(TimeSpan delta)
        {
            _remainTime -= delta;

            if (_remainTime.TotalSeconds <= 0)
            {
                RandFood();
            }
        }

        public void OnCollisionEnter(GameObject gameObject)
        {
            if (gameObject.Name == "Snake head")
                RandFood();
        }

        private void RandFood()
        {
            int number = _random.Next(0, 11);

            if (number > 4)
            {
                _activeFoodIndex = 0;
            }
            else if (number is > 0 and <= 4)
            {
                _activeFoodIndex = _random.Next(1, _foods.Length - 1);
            }
            else if (number == 0)
            {
                _activeFoodIndex = ^1;
            }

            while (true)
            {
                Vector2 position = new(_random.Next(1, _grid.Size.X - 1), _random.Next(1, _grid.Size.Y - 1));
                if (_grid.IsPositionOccupied(position, foodScale) is false)
                {
                    ActiveFood.GetComponent<Transform>().Position = position;
                    break;
                }
            }

            _remainTime = TimeSpan.FromSeconds(ActiveFood.LifeTime);
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            yield return ActiveFood;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return ActiveFood;
        }
    }
}