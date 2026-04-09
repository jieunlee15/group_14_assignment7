// Group 14:
// group member 1: tanmayee bharadwaj; tpb675
// group member 2: nicco faelnar; nvf89
// group member 3: jieun lee; jl83729

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_14_assignment7
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private Player _player;
        private Texture2D _duckIdle;
        private Texture2D _duckHop;
        
        private Texture2D _gridTexture;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load duck sprites
            _duckIdle = Content.Load<Texture2D>("duck_idle");
            _duckHop = Content.Load<Texture2D>("duck_hop");
            
            // Create player
            _player = new Player(_duckIdle, _duckHop, new Vector2(5, 10));
            
            // Create simple grid texture
            CreateGridTexture();
        }

        private void CreateGridTexture()
        {
            _gridTexture = new Texture2D(GraphicsDevice, 64, 64);
            Color[] data = new Color[64 * 64];
            
            for (int i = 0; i < data.Length; i++)
            {
                int x = i % 64;
                int y = i / 64;
                
                // Draw border
                if (x == 0 || x == 63 || y == 0 || y == 63)
                {
                    data[i] = Color.Gray * 0.5f;
                }
                else
                {
                    data[i] = Color.DarkGreen * 0.3f;
                }
            }
            
            _gridTexture.SetData(data);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            _spriteBatch.Begin();
            
            // Draw grid
            DrawGrid();
            
            // Draw player
            _player.Draw(_spriteBatch);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            int gridSize = 64;
            int offsetX = 400;
            int offsetY = 100;
            
            // Draw 10x15 grid
            for (int y = 0; y < 15; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Vector2 position = new Vector2(offsetX + x * gridSize, offsetY + y * gridSize);
                    _spriteBatch.Draw(_gridTexture, position, Color.White);
                }
            }
        }
    }
}