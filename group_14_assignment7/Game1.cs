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
    public enum GameState
    {
        Playing,
        GameOver,
        Victory
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        private Player _player;
        private Texture2D _duckIdle;
        private Texture2D _duckHop;
        
        private Texture2D _gridTexture;
        private Texture2D _pixelTexture;

        private Vehicle[] vehicles;
        
        // Game State
        private GameState _currentState;
        
        // Game Logic
        private int score;
        private int highScore;
        private int farthestThisScreen;
        private const int WIN_SCORE = 50; // Win condition: reach score of 50
        
        private string scoreText = "Score: 0";
        private SpriteFont _font;
        private Vector2 scorePosition;
        private Color scoreColor = Color.White;
        
        // Input
        private KeyboardState _previousKeyState;

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
            highScore = 0;
            farthestThisScreen = 10;
            _currentState = GameState.Playing;
            _previousKeyState = Keyboard.GetState();
            
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
            CreatePixelTexture();
            
            // Load font
            _font = Content.Load<SpriteFont>("font/fredokaOneRegular");
            scoreText = "Score: " + score.ToString();
            scorePosition = new Vector2(5, 5);
            
            // Load vehicle
            vehicles = new Vehicle[1];
            vehicles[0] = new Vehicle(
                Content.Load<Texture2D>("vehicles/blueCar"),
                new Vector2(700, 200),
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

        private void CreatePixelTexture()
        {
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            
            if (currentKeyState.IsKeyDown(Keys.Escape))
                Exit();

            switch (_currentState)
            {
                case GameState.Playing:
                    UpdatePlaying(gameTime);
                    break;
                case GameState.GameOver:
                    UpdateGameOver(currentKeyState);
                    break;
                case GameState.Victory:
                    UpdateVictory(currentKeyState);
                    break;
            }

            _previousKeyState = currentKeyState;
            base.Update(gameTime);
        }

        private void UpdatePlaying(GameTime gameTime)
        {
            _player.Update(gameTime);
            CheckPlayerCollision();
            UpdateScore();
            CheckPlayerOffScreen();
            CheckWinCondition();

            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Move();
            }
        }

        private void UpdateGameOver(KeyboardState currentKeyState)
        {
            // Press Space to restart
            if (currentKeyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space))
            {
                RestartGame();
            }
        }

        private void UpdateVictory(KeyboardState currentKeyState)
        {
            // Press Space to restart
            if (currentKeyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space))
            {
                RestartGame();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);

            _spriteBatch.Begin();
            
            switch (_currentState)
            {
                case GameState.Playing:
                    DrawPlaying();
                    break;
                case GameState.GameOver:
                    DrawGameOver();
                    break;
                case GameState.Victory:
                    DrawVictory();
                    break;
            }
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawPlaying()
        {
            // Draw grid
            DrawGrid();
            
            // Draw vehicles
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.Draw(_spriteBatch);
            }
            
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
        }

        private void DrawGameOver()
        {
            // Semi-transparent dark overlay
            Rectangle overlay = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _spriteBatch.Draw(_pixelTexture, overlay, Color.Black * 0.8f);
            
            // Title
            string title = "GAME OVER";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - titleSize.X) / 2,
                150
            );
            _spriteBatch.DrawString(_font, title, titlePosition, Color.Red);
            
            // Final Score
            string finalScoreText = "Final Score: " + score;
            Vector2 scoreSize = _font.MeasureString(finalScoreText);
            Vector2 finalScorePosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - scoreSize.X) / 2,
                250
            );
            _spriteBatch.DrawString(_font, finalScoreText, finalScorePosition, Color.White);
            
            // High Score
            string highScoreText = "High Score: " + highScore;
            Vector2 highScoreSize = _font.MeasureString(highScoreText);
            Vector2 highScorePosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - highScoreSize.X) / 2,
                320
            );
            _spriteBatch.DrawString(_font, highScoreText, highScorePosition, Color.Yellow);
            
            // Instructions
            string restartText = "Press SPACE to Restart";
            Vector2 restartSize = _font.MeasureString(restartText);
            Vector2 restartPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - restartSize.X) / 2,
                450
            );
            _spriteBatch.DrawString(_font, restartText, restartPosition, Color.LightGray);
            
            string quitText = "Press ESCAPE to Quit";
            Vector2 quitSize = _font.MeasureString(quitText);
            Vector2 quitPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - quitSize.X) / 2,
                510
            );
            _spriteBatch.DrawString(_font, quitText, quitPosition, Color.LightGray);
        }

        private void DrawVictory()
        {
            // Semi-transparent overlay
            Rectangle overlay = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _spriteBatch.Draw(_pixelTexture, overlay, Color.Black * 0.7f);
            
            // Title
            string title = "VICTORY!";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - titleSize.X) / 2,
                150
            );
            _spriteBatch.DrawString(_font, title, titlePosition, Color.Gold);
            
            // Congratulations message
            string congrats = "You reached " + WIN_SCORE + " points!";
            Vector2 congratsSize = _font.MeasureString(congrats);
            Vector2 congratsPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - congratsSize.X) / 2,
                250
            );
            _spriteBatch.DrawString(_font, congrats, congratsPosition, Color.White);
            
            // Final Score
            string finalScoreText = "Final Score: " + score;
            Vector2 scoreSize = _font.MeasureString(finalScoreText);
            Vector2 finalScorePosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - scoreSize.X) / 2,
                320
            );
            _spriteBatch.DrawString(_font, finalScoreText, finalScorePosition, Color.LightGreen);
            
            // Instructions
            string restartText = "Press SPACE to Play Again";
            Vector2 restartSize = _font.MeasureString(restartText);
            Vector2 restartPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - restartSize.X) / 2,
                450
            );
            _spriteBatch.DrawString(_font, restartText, restartPosition, Color.LightGray);
            
            string quitText = "Press ESCAPE to Quit";
            Vector2 quitSize = _font.MeasureString(quitText);
            Vector2 quitPosition = new Vector2(
                (_graphics.PreferredBackBufferWidth - quitSize.X) / 2,
                510
            );
            _spriteBatch.DrawString(_font, quitText, quitPosition, Color.LightGray);
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
                    // Player got hit - Game Over!
                    _player.IsAlive = false;
                    _currentState = GameState.GameOver;
                    
                    // Update high score
                    if (score > highScore)
                    {
                        highScore = score;
                    }
                    
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
                
                // Place new vehicle lanes (TODO: add more vehicles)
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

        private void CheckWinCondition()
        {
            if (score >= WIN_SCORE)
            {
                _currentState = GameState.Victory;
                
                // Update high score
                if (score > highScore)
                {
                    highScore = score;
                }
            }
        }

        private void RestartGame()
        {
            // Reset score
            score = 0;
            farthestThisScreen = 10;
            scoreText = "Score: " + score.ToString();
            
            // Reset player
            _player.Reset();
            
            // Reset vehicles (recreate them at starting positions)
            vehicles[0] = new Vehicle(
                Content.Load<Texture2D>("vehicles/blueCar"),
                new Vector2(700, 200),
                new Vector2(-5, 0),
                _player,
                new Vector2(Window.ClientBounds.X, Window.ClientBounds.Y),
                true,
                0.2f
            );
            
            // Return to playing state
            _currentState = GameState.Playing;
        }
    }
}