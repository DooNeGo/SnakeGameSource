namespace SnakeGameSource.GameEngine;

internal static class ObjectsTree
{
    private static readonly List<GameObject> GameObjects = [];

    public static void Add(GameObject gameObject)
    {
        GameObjects.Add(gameObject);
    }

    public static IEnumerable<GameObject> GetGameObjects()
    {
        return GameObjects.AsEnumerable();
    }
}