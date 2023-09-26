using System.Collections;
using System.Collections.Generic;

namespace SnakeGameSource.Model
{
    internal class Scene : IEnumerable<GameObject>
    {
        private readonly IEnumerable<GameObject>[] _compositeObjects;
        private readonly List<GameObject> _gameObjects = new();

        public Scene(params IEnumerable<GameObject>[] compositeObjects)
        {
            _compositeObjects = compositeObjects;
            Update();
        }

        public void Update()
        {
            _gameObjects.Clear();

            for (int i = 0; i < _compositeObjects.Length; i++)
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