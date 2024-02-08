namespace SnakeGameSource.GameEngine.Abstractions;

internal interface ISpriteDrawer
{
    public void Draw();

    public void LoadContent();

    public void UnloadContent();
}