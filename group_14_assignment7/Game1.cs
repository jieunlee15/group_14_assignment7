// Group 14:
// group member 1: tanmayee bharadwaj; tpb675
// group member 2: nicco faelnar; nvf89
// group member 3: jieun lee; jl83729

using System;
using System.Diagnostics;
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

        private Vehicle[] vehicles;
        
        // Game Logic
        private int score;
        private int farthestThisScreen;
        
        private string scoreText = "Score: 0";
        private SpriteFont _font;
        private Vector2 scorePosition;
        private Color scoreColor = Color.White;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.PreferredBackBufferWidth = 640;
            _graphics.PreferredBackBufferHeight = 705;
        }

        protected override void Initialize()
        {
            score = 0;
            farthestThisScreen = 10;
            
            
            
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
            
            // Load font
            _font = Content.Load<SpriteFont>("font/fredokaOneRegular");
            scoreText = "Score: " + score.ToString();
            scorePosition = new Vector2(5, 5);
            
            // Load vehicle
            vehicles = new Vehicle[1];
            vehicles[0] = new Vehicle(
                Content.Load<Texture2D>("vehicles/blueCar"),
                new  Vector2(700, 200),
                new Vector2(-5, 0),
                _player,
                new Vector2(Window.ClientBounds.X, Window.ClientBounds.Y),
                true,
                0.2f
            );
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
            CheckPlayerCollision();
            UpdateScore();
            CheckPlayerOffScreen();

            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Move();
            }

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
            
            // Draw score
            _spriteBatch.DrawString(
                _font,
                scoreText,
                scorePosition,
                scoreColor,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None,
                0
            );

            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Draw(_spriteBatch);
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawGrid()
        {
            int gridSize = 64;
            int offsetX = 0;
            int offsetY = 0;
            
            // Draw 10x11 grid
            for (int y = 0; y < 11; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Vector2 position = new Vector2(offsetX + x * gridSize, offsetY + y * gridSize);
                    _spriteBatch.Draw(_gridTexture, position, Color.White);
                }
            }
        }

        private void CheckPlayerCollision()
        {
            foreach (Vehicle vehicle in vehicles)
            {
                if (_player.GetBounds().Intersects(vehicle.GetCollider()))
                {
                    // Player got hit, you lose screen
                    Console.WriteLine("YEAHOWOHCH");
                    return;
                }
            }
        }

        private void CheckPlayerOffScreen()
        {
            if (_player.PixelPosition.Y < 0 && !_player._isMoving)
            {
                // Put player back at the bottom of the screen
                _player.ResetPosition();
                // Reset farthest
                farthestThisScreen = 10;
                
                // Place new vehicle lanes
            }
        }

        private void UpdateScore()
        {
            if (_player.GridPosition.Y < farthestThisScreen)
            {
                score++;
                farthestThisScreen--;
                scoreText = "Score: " + score.ToString();
            }
        }
    }
}