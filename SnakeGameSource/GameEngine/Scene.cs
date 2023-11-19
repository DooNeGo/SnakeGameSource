using SnakeGameSource.GameEngine.Components;
using System.Collections;
using System.Reflection;

namespace SnakeGameSource.GameEngine
{
    public class Scene : IEnumerable<GameObject>
    {
        private static readonly string UpdateMethodName = "Update";

        private readonly List<IEnumerable<GameObject>> _compositeObjects = [];
        private readonly List<GameObject> _gameObjects = [];

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

        public void Update(TimeSpan delta)
        {
            UpdateGameObjectsList();
            InvokeUpdateMethods(delta);
        }

        private void InvokeUpdateMethods(TimeSpan delta)
        {
            for (int i = 0; i < _gameObjects.Count; i++)
            {
                foreach (Component component in _gameObjects[i].GetComponents())
                {
                    Type type = component.GetType();
                    MethodInfo? method = type.GetMethod(UpdateMethodName,
                        BindingFlags.Instance | BindingFlags.NonPublic,
                        [typeof(TimeSpan)]);
                    method?.Invoke(component, [delta]);
                }
            }
        }

        private void UpdateGameObjectsList()
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