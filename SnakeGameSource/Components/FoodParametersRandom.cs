using SnakeGameSource.GameEngine;
using SnakeGameSource.GameEngine.Components;

namespace SnakeGameSource.Components
{
    internal class FoodParametersRandom : Component
    {
        private readonly Random _random = new();

        private Effect[]? _effects;
        private TimeSpan _remainLifetime;

        public Grid? Grid { get; set; }

        public int FoodLifetime { get; set; }

        private void Awake()
        {
            _effects = new Effect[6];

            for (int i = 0; i < _effects.Length; i++)
            {
                _effects[i] = new Effect();
            }

            _effects[0].Type = EffectType.Length;
            _effects[0].Value = 1;

            _effects[1].Type = EffectType.Speed;
            _effects[1].Value = 0.5f;

            _effects[2].Type = EffectType.Speed;
            _effects[2].Value = 0.5f;

            _effects[3].Type = EffectType.Scale;
            _effects[3].Value = 0.1f;

            _effects[4].Type = EffectType.Scale;
            _effects[4].Value = -0.1f;

            _effects[5].Type = EffectType.Length;
            _effects[5].Value = -1;
        }

        private void Update(TimeSpan delta)
        {
            _remainLifetime -= delta;

            if (_remainLifetime.TotalSeconds <= 0)
                RandFoodParameters();
        }

        private void OnCollisionEnter(GameObject gameObject)
        {
            if (gameObject.Name is "Snake head")
                RandFoodParameters();
        }

        private void RandFoodParameters()
        {
            RandEffect();
            RandPosition();

            _remainLifetime = TimeSpan.FromSeconds(FoodLifetime);
        }

        private void RandEffect()
        {
            if (_effects is null)
                throw new NullReferenceException(nameof(_effects));

            int number = _random.Next(0, _effects.Length);
            SetEffect(_effects[number]);
        }

        private void SetEffect(Effect effect)
        {
            effect.CopyTo(GetComponent<Effect>());
        }

        private void RandPosition()
        {
            if (Grid is null)
                throw new NullReferenceException(nameof(Grid) + "must be not null");

            Transform transform = GetComponent<Transform>();
            do
            {
                transform.Position = new(_random.Next(1, Grid.Size.X - 1),
                                         _random.Next(1, Grid.Size.Y - 1));
            } while (Grid.IsPositionOccupied(transform.Position, transform.Scale));
        }
    }
}
