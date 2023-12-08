using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnakeGameSource.GameEngine;

public class Game2D : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private CollisionHandler _collisionHandler;
    private SpriteDrawer     _drawer;

    protected Game2D()
    {
        _graphics             = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible        = true;
    }

    protected Input Input { get; private set; }

    protected DiContainer Container { get; } = new();

    protected Scene Scene { get; set; }

    protected Color BackgroudColor { get; set; } = Color.MediumPurple;

    protected Grid Grid { get; private set; }

    public event Action?           Initializing;
    public event Action?           LoadingContent;
    public event Action?           UnloadingContent;
    public event Action<GameTime>? Updating;
    public event Action<GameTime>? Drawing;

    protected override void Initialize()
    {
        Container.AddSingleton<Input>()
                 .AddSingleton<SpriteDrawer>()
                 .AddSingleton<CollisionHandler>()
                 .AddSingleton<Scene>()
                 .AddSingleton<Grid>()
                 .AddSingleton<SpriteBatch>()
                 .AddSingleton(Window)
                 .AddSingleton(Content)
                 .AddSingleton(GraphicsDevice)
                 .AddSingleton(Container)
                 .Build();

        _collisionHandler = Container.GetInstance<CollisionHandler>();
        _drawer           = Container.GetInstance<SpriteDrawer>();
        Input             = Container.GetInstance<Input>();
        Scene             = Container.GetInstance<Scene>();
        Grid              = Container.GetInstance<Grid>();

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
        if (!IsActive)
        {
            return;
        }

        Scene.Update(gameTime.ElapsedGameTime);
        _collisionHandler.Update();
        Input.Update();
        Updating?.Invoke(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(BackgroudColor);
        _drawer.Draw();
        Drawing?.Invoke(gameTime);

        base.Draw(gameTime);
    }
}