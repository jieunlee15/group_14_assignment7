// Group 14:
// group member 1: tanmayee bharadwaj; tpb675
// group member 2: nicco faelnar; nvf89
// group member 3: jieun lee; jl83729

using System;
using System.Collections.Generic;
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

        private Texture2D _grassTexture;
        private Texture2D _roadTexture;
        private Texture2D _pixelTexture;

        private Lane[] _lanes;

        // Game State
        private GameState _currentState;

        // Game Logic
        private int score;
        private int highScore;
        private int farthestThisScreen;
        private const int WIN_SCORE = 50;

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

            _duckIdle = Content.Load<Texture2D>("duck_idle");
            _duckHop = Content.Load<Texture2D>("duck_hop");

            _player = new Player(_duckIdle, _duckHop, new Vector2(5, 10));

            _grassTexture = CreateGrassTexture();
            _roadTexture = CreateRoadTexture();
            _pixelTexture = CreatePixelTexture();

            _font = Content.Load<SpriteFont>("font/fredokaOneRegular");
            scoreText = "Score: " + score.ToString();
            scorePosition = new Vector2(5, 5);

            Texture2D carTexture = Content.Load<Texture2D>("vehicles/blueCar");
            InitializeLanes(carTexture);
        }

        private void InitializeLanes(Texture2D carTexture)
        {
            int screenWidth = _graphics.PreferredBackBufferWidth;

            // 11 rows: row 0 = safe start (grass), rows 1-9 = road lanes, row 10 = safe end (grass)
            _lanes = new Lane[11];

            _lanes[0] = new Lane(0, LaneType.Grass, Direction.Left, 0, 999f, _grassTexture, null, _player, screenWidth);
            _lanes[10] = new Lane(10, LaneType.Grass, Direction.Left, 0, 999f, _grassTexture, null, _player, screenWidth);

            for (int i = 1; i <= 9; i++)
            {
                Direction dir = (i % 2 == 0) ? Direction.Left : Direction.Right;
                float speed = 2.5f + (i * 0.3f);
                float interval = Math.Max(3.0f, 6.0f - (i * 0.2f));

                _lanes[i] = new Lane(i, LaneType.Road, dir, speed, interval, _roadTexture, carTexture, _player, screenWidth);
            }
        }

        private Texture2D CreateGrassTexture()
        {
            Texture2D tex = new Texture2D(GraphicsDevice, 64, 64);
            Color[] data = new Color[64 * 64];
            for (int i = 0; i < data.Length; i++)
            {
                int x = i % 64;
                int y = i / 64;
                data[i] = (x == 0 || x == 63 || y == 0 || y == 63)
                    ? Color.Gray * 0.5f
                    : Color.DarkGreen * 0.3f;
            }
            tex.SetData(data);
            return tex;
        }

        private Texture2D CreateRoadTexture()
        {
            Texture2D tex = new Texture2D(GraphicsDevice, 64, 64);
            Color[] data = new Color[64 * 64];
            for (int i = 0; i < data.Length; i++)
            {
                int x = i % 64;
                int y = i / 64;
                data[i] = (x == 0 || x == 63 || y == 0 || y == 63)
                    ? Color.DarkGray * 0.8f
                    : Color.Gray * 0.5f;
            }
            tex.SetData(data);
            return tex;
        }

        private Texture2D CreatePixelTexture()
        {
            Texture2D tex = new Texture2D(GraphicsDevice, 1, 1);
            tex.SetData(new[] { Color.White });
            return tex;
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

            foreach (Lane lane in _lanes)
                lane.Update(gameTime);

            CheckPlayerCollision();
            UpdateScore();
            CheckPlayerOffScreen();
            CheckWinCondition();
        }

        private void UpdateGameOver(KeyboardState currentKeyState)
        {
            if (currentKeyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space))
                RestartGame();
        }

        private void UpdateVictory(KeyboardState currentKeyState)
        {
            if (currentKeyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space))
                RestartGame();
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
            foreach (Lane lane in _lanes)
                lane.Draw(_spriteBatch);

            _player.Draw(_spriteBatch);

            _spriteBatch.DrawString(_font, scoreText, scorePosition, scoreColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        private void DrawGameOver()
        {
            Rectangle overlay = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _spriteBatch.Draw(_pixelTexture, overlay, Color.Black * 0.8f);

            string title = "GAME OVER";
            Vector2 titleSize = _font.MeasureString(title);
            _spriteBatch.DrawString(_font, title, new Vector2((_graphics.PreferredBackBufferWidth - titleSize.X) / 2, 150), Color.Red);

            string finalScoreText = "Final Score: " + score;
            Vector2 scoreSize = _font.MeasureString(finalScoreText);
            _spriteBatch.DrawString(_font, finalScoreText, new Vector2((_graphics.PreferredBackBufferWidth - scoreSize.X) / 2, 250), Color.White);

            string highScoreText = "High Score: " + highScore;
            Vector2 highScoreSize = _font.MeasureString(highScoreText);
            _spriteBatch.DrawString(_font, highScoreText, new Vector2((_graphics.PreferredBackBufferWidth - highScoreSize.X) / 2, 320), Color.Yellow);

            string restartText = "Press SPACE to Restart";
            Vector2 restartSize = _font.MeasureString(restartText);
            _spriteBatch.DrawString(_font, restartText, new Vector2((_graphics.PreferredBackBufferWidth - restartSize.X) / 2, 450), Color.LightGray);

            string quitText = "Press ESCAPE to Quit";
            Vector2 quitSize = _font.MeasureString(quitText);
            _spriteBatch.DrawString(_font, quitText, new Vector2((_graphics.PreferredBackBufferWidth - quitSize.X) / 2, 510), Color.LightGray);
        }

        private void DrawVictory()
        {
            Rectangle overlay = new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _spriteBatch.Draw(_pixelTexture, overlay, Color.Black * 0.7f);

            string title = "VICTORY!";
            Vector2 titleSize = _font.MeasureString(title);
            _spriteBatch.DrawString(_font, title, new Vector2((_graphics.PreferredBackBufferWidth - titleSize.X) / 2, 150), Color.Gold);

            string congrats = "You reached " + WIN_SCORE + " points!";
            Vector2 congratsSize = _font.MeasureString(congrats);
            _spriteBatch.DrawString(_font, congrats, new Vector2((_graphics.PreferredBackBufferWidth - congratsSize.X) / 2, 250), Color.White);

            string finalScoreText = "Final Score: " + score;
            Vector2 scoreSize = _font.MeasureString(finalScoreText);
            _spriteBatch.DrawString(_font, finalScoreText, new Vector2((_graphics.PreferredBackBufferWidth - scoreSize.X) / 2, 320), Color.LightGreen);

            string restartText = "Press SPACE to Play Again";
            Vector2 restartSize = _font.MeasureString(restartText);
            _spriteBatch.DrawString(_font, restartText, new Vector2((_graphics.PreferredBackBufferWidth - restartSize.X) / 2, 450), Color.LightGray);

            string quitText = "Press ESCAPE to Quit";
            Vector2 quitSize = _font.MeasureString(quitText);
            _spriteBatch.DrawString(_font, quitText, new Vector2((_graphics.PreferredBackBufferWidth - quitSize.X) / 2, 510), Color.LightGray);
        }

        private void CheckPlayerCollision()
        {
            foreach (Lane lane in _lanes)
            {
                foreach (Vehicle vehicle in lane.GetActiveVehicles())
                {
                    if (_player.GetBounds().Intersects(vehicle.GetCollider()))
                    {
                        _player.IsAlive = false;
                        _currentState = GameState.GameOver;

                        if (score > highScore)
                            highScore = score;

                        return;
                    }
                }
            }
        }

        private void CheckPlayerOffScreen()
        {
            if (_player.PixelPosition.Y < 0 && !_player._isMoving)
            {
                _player.ResetPosition();
                farthestThisScreen = 10;
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

                if (score > highScore)
                    highScore = score;
            }
        }

        private void RestartGame()
        {
            score = 0;
            farthestThisScreen = 10;
            scoreText = "Score: " + score.ToString();

            _player.Reset();

            Texture2D carTexture = Content.Load<Texture2D>("vehicles/blueCar");
            InitializeLanes(carTexture);

            _currentState = GameState.Playing;
        }
    }
}