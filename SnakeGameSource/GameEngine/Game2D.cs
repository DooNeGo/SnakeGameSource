using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SnakeGameSource.GameEngine.Abstractions;

namespace SnakeGameSource.GameEngine;

public class Game2D : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private ICollisionHandler _collisionHandler;
    private SpriteDrawer      _drawer;

    protected Game2D()
    {
        _graphics             = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible        = true;
    }

    protected IInput Input => Container.GetInstance<IInput>();

    protected DiContainer Container { get; } = new();

    protected IScene Scene { get; private set; }

    protected Color BackgroundColor { get; set; }

    protected IGrid Grid { get; private set; }

    protected bool IsStop { get; set; }

    protected float TimeRatio { get; set; } = 1f;

    public event Action? Initializing;

    public event Action? LoadingContent;

    public event Action? UnloadingContent;

    public event Action<GameTime>? Updating;

    public event Action<GameTime>? Drawing;

    protected override void Initialize()
    {
        Container.AddSingleton<IInput, Bot>()
                 .AddSingleton<SpriteDrawer>()
                 .AddSingleton<ICollisionHandler, CollisionHandler>()
                 .AddSingleton<IScene, Scene>()
                 .AddSingleton<IGrid, Grid>()
                 .AddSingleton<SpriteBatch>()
                 .AddSingleton(Window)
                 .AddSingleton(Content)
                 .AddSingleton(GraphicsDevice)
                 .AddSingleton(Container);

        _collisionHandler = Container.GetInstance<ICollisionHandler>();
        _drawer           = Container.GetInstance<SpriteDrawer>();

        //Input             = Container.GetInstance<IInput>();
        Scene = Container.GetInstance<IScene>();
        Grid  = Container.GetInstance<IGrid>();

        Initializing?.Invoke();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _drawer.LoadContent();
        LoadingContent?.Invoke();

        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _drawer.UnloadContent();
        UnloadingContent?.Invoke();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        gameTime.ElapsedGameTime *= TimeRatio;

        if (!IsActive)
        {
            return;
        }

        Input.Update();

        if (!IsStop)
        {
            Scene.Update(gameTime.ElapsedGameTime);
            _collisionHandler.Update();

            Updating?.Invoke(gameTime);
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(BackgroundColor);
        _drawer.Draw();

        gameTime.ElapsedGameTime *= TimeRatio;
        Drawing?.Invoke(gameTime);

        base.Draw(gameTime);
    }
}