namespace SnakeGameSource.GameEngine.Exceptions;

internal sealed class ComponentNotFoundException
    (string componentName) : Exception($"The component: {componentName} was not found in gameObject")
{
}