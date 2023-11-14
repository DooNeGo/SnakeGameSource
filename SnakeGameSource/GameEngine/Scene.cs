using System.Collections;

namespace SnakeGameSource.GameEngine
{
    internal class Scene : IEnumerable<GameObject>
    {
        private readonly List<IEnumerable<GameObject>> _compositeObjects = new();
        private readonly List<GameObject> _gameObjects = new();

        public void Add(params IEnumerable<GameObject>[] compositeObjects)
        {
            _compositeObjects.AddRange(compositeObjects);
        }

        public void Remove(params IEnumerable<GameObject>[] compositeObjects)
        {
            for (int i = 0; i < compositeObjects.Length; i++)
            {
                _compositeObjects.Remove(compositeObjects[i]);
            }
        }

        public void Update()
        {
            _gameObjects.Clear();

            for (int i = 0; i < _compositeObjects.Count; i++)
            {
                _gameObjects.AddRange(_compositeObjects[i]);
            }
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return _gameObjects.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _gameObjects.GetEnumerator();
        }
    }
}