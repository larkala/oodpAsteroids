using AsteroidsApp.Messages;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AsteroidsApp
{
    public class Asteroids : Game
    {
        GameState state;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        SpriteFont bigFontTexture, statsFont;

        Player player;
        GameObjectManager gameObjectManager;
        KeyboardState previousKbState;

        public Asteroids()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = Globals.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Globals.ScreenHeight;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            //Create a player with this game as a parameter.
            player = new Player(this);

            Components.Add(player);

            gameObjectManager = new GameObjectManager(this);
            Components.Add(gameObjectManager);

            Mediator.Instance.Register<GameStateChangedMessage>(this, OnGameStateChangedCallback);
            Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.GetReady });
            base.Initialize();
        }

        //Meddelandehantering om ändring på tillstånd (gamestate)
        private void OnGameStateChangedCallback(GameStateChangedMessage message)
        {
            if (message.NewState == state)
                return;
            switch (message.NewState)
            {
                case GameState.GetReady:
                    gameObjectManager.Init();
                    player.Init();
                    StatsSingleton.Instance.LevelUp();
                    player.Enabled = gameObjectManager.Enabled = false;
                    break;
                case GameState.Playing:
                    player.Enabled = gameObjectManager.Enabled = true;
                    break;
                case GameState.Dead:
                case GameState.Win:
                    player.Enabled = gameObjectManager.Enabled = false;
                    break;
            }
            state = message.NewState;
        }
        
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            bigFontTexture = Content.Load<SpriteFont>("spriteFontBig");
            statsFont = Content.Load<SpriteFont>("Score");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState kbState = Keyboard.GetState();

            switch (state)
            {
                case GameState.Dead:
                    StatsSingleton.Instance.Reset();
                    if (kbState.IsKeyDown(Keys.Enter) && previousKbState.IsKeyUp(Keys.Enter))
                        Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.GetReady });
                    break;
                case GameState.Win:
                    if (kbState.IsKeyDown(Keys.Enter) && previousKbState.IsKeyUp(Keys.Enter))
                        Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.GetReady });
                    break;
                case GameState.GetReady:
                    if (kbState.IsKeyDown(Keys.Enter) && previousKbState.IsKeyUp(Keys.Enter))
                        Mediator.Instance.Send(new GameStateChangedMessage() { NewState = GameState.Playing });
                    break;
                case GameState.Playing:
                    gameObjectManager.CheckPlayerCollision(player);
                    break;
            }
            previousKbState = kbState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            gameObjectManager.Draw(_spriteBatch);
           
            player.Draw(_spriteBatch);


            string overlayText = null;
            switch(state)
            {
                case GameState.GetReady:
                    overlayText = "get ready! press enter!";
                    break;
                case GameState.Dead:
                    overlayText = "game over";
                    break;
                case GameState.Win:
                    overlayText = $"Level {StatsSingleton.Instance.Level} Cleared!";
                    break;

            }

            if(!string.IsNullOrEmpty(overlayText))
            {
                var size1 = bigFontTexture.MeasureString(overlayText);
                _spriteBatch.DrawString(bigFontTexture, overlayText, Globals.ScreenCenter - size1/2.0f, Color.White);
            }

            string pointText = $"POINTS: {StatsSingleton.Instance.Points}";
            _spriteBatch.DrawString(statsFont, pointText, new Vector2(20, 10), Color.White);
            
            string levelText = $"LEVEL: {StatsSingleton.Instance.Level}";
            var size2 = statsFont.MeasureString(levelText);
            _spriteBatch.DrawString(statsFont, levelText, new Vector2(Globals.ScreenCenter.X-(size2.X)/2.0f, 10), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
