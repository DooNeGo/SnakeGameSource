using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SnakeGameSource.GameEngine;

public class Game2D : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private CollisionHandler _collisionHandler;
    private SpriteDrawer     _drawer;

    private float _value;

    protected Game2D()
    {
        _graphics             = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible        = true;
    }

    protected Input Input { get; private set; }

    protected DiContainer Container { get; } = new();

    protected Scene Scene { get; set; }

    protected Color BackgroundColor1 { get; set; } = new(224, 172, 213);

    protected Color BackgroundColor2 { get; set; } = new(57, 147, 221);

    protected Grid Grid { get; private set; }

    protected bool IsStop { get; set; } = false;

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
        GraphicsDevice.Clear(Color.Lerp(BackgroundColor1, BackgroundColor2, MathF.Cos(_value)));
        _drawer.Draw();

        if (!IsStop)
        {
            Drawing?.Invoke(gameTime);
            _value += 0.005f;
        }

        base.Draw(gameTime);
    }
}