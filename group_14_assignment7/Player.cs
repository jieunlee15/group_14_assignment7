// Jieun Lee
// eid: jl83729
// Group 14 - Assignment 7

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace group_14_assignment7
{
    public class Player
    {
        // Grid and position
        public Vector2 GridPosition { get; set; }
        public Vector2 PixelPosition { get; private set; }
        
        private const int GRID_SIZE = 64;
        private const int SCREEN_OFFSET_X = 0;
        private const int SCREEN_OFFSET_Y = 0;
        
        // Animation
        private int _animationFrame;
        private float _animationTimer;
        private const float ANIMATION_SPEED = 0.15f;
        
        // Movement
        public bool _isMoving;
        private Vector2 _targetGridPosition;
        private float _moveTimer;
        private const float MOVE_DURATION = 0.2f;
        
        // Sprites
        private Texture2D _idleSprite;
        private Texture2D _hopSprite;
        
        // State
        public bool IsAlive { get; set; }
        private KeyboardState _previousKeyState;
        
        public Player(Texture2D idleSprite, Texture2D hopSprite, Vector2 startGridPosition)
        {
            _idleSprite = idleSprite;
            _hopSprite = hopSprite;
            
            GridPosition = startGridPosition;
            PixelPosition = GridToPixel(GridPosition);
            
            _animationFrame = 0;
            _animationTimer = 0f;
            
            _isMoving = false;
            _moveTimer = 0f;
            
            IsAlive = true;
            _previousKeyState = Keyboard.GetState();
        }

        public void Update(GameTime gameTime)
        {
            if (!IsAlive) return;
            
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Handle input only when not moving
            if (!_isMoving)
            {
                HandleInput();
            }
            
            // Update movement animation
            if (_isMoving)
            {
                UpdateMovement(deltaTime);
            }
            
            // Update animation timer
            UpdateAnimation(deltaTime);
        }

        private void HandleInput()
        {
            KeyboardState currentKeyState = Keyboard.GetState();
            
            // Check for new key presses (not held down)
            bool upPressed = currentKeyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up);
            bool downPressed = currentKeyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down);
            bool leftPressed = currentKeyState.IsKeyDown(Keys.Left) && !_previousKeyState.IsKeyDown(Keys.Left);
            bool rightPressed = currentKeyState.IsKeyDown(Keys.Right) && !_previousKeyState.IsKeyDown(Keys.Right);
            
            bool wPressed = currentKeyState.IsKeyDown(Keys.W) && !_previousKeyState.IsKeyDown(Keys.W);
            bool sPressed = currentKeyState.IsKeyDown(Keys.S) && !_previousKeyState.IsKeyDown(Keys.S);
            bool aPressed = currentKeyState.IsKeyDown(Keys.A) && !_previousKeyState.IsKeyDown(Keys.A);
            bool dPressed = currentKeyState.IsKeyDown(Keys.D) && !_previousKeyState.IsKeyDown(Keys.D);
            
            // Move forward (up)
            if (upPressed || wPressed)
            {
                Vector2 newPos = GridPosition + new Vector2(0, -1);
                MoveToGridPosition(newPos);
            }
            // Move backward (down)
            else if (downPressed || sPressed)
            {
                Vector2 newPos = GridPosition + new Vector2(0, 1);
                MoveToGridPosition(newPos);
            }
            // Move left
            else if (leftPressed || aPressed)
            {
                Vector2 newPos = GridPosition + new Vector2(-1, 0);
                MoveToGridPosition(newPos);
            }
            // Move right
            else if (rightPressed || dPressed)
            {
                Vector2 newPos = GridPosition + new Vector2(1, 0);
                MoveToGridPosition(newPos);
            }
            
            _previousKeyState = currentKeyState;
        }

        private void MoveToGridPosition(Vector2 targetGrid)
        {
            // Add boundary checking here if needed
            // For now, allow any movement
            
            _targetGridPosition = targetGrid;
            _isMoving = true;
            _moveTimer = 0f;
        }

        private void UpdateMovement(float deltaTime)
        {
            _moveTimer += deltaTime;
            
            float t = _moveTimer / MOVE_DURATION;
            
            if (t >= 1.0f)
            {
                // Movement complete
                GridPosition = _targetGridPosition;
                PixelPosition = GridToPixel(GridPosition);
                _isMoving = false;
                _moveTimer = 0f;
            }
            else
            {
                // Lerp between current and target position
                Vector2 startPixel = GridToPixel(GridPosition);
                Vector2 targetPixel = GridToPixel(_targetGridPosition);
                PixelPosition = Vector2.Lerp(startPixel, targetPixel, t);
            }
        }

        private void UpdateAnimation(float deltaTime)
        {
            _animationTimer += deltaTime;
            
            if (_animationTimer >= ANIMATION_SPEED)
            {
                _animationFrame++;
                _animationTimer = 0f;
            }
        }

        private Vector2 GridToPixel(Vector2 gridPos)
        {
            return new Vector2(
                SCREEN_OFFSET_X + gridPos.X * GRID_SIZE,
                SCREEN_OFFSET_Y + gridPos.Y * GRID_SIZE
            );
        }

        public Rectangle GetBounds()
        {
            // Collision box (slightly smaller than sprite for better gameplay)
            int boxSize = GRID_SIZE - 10;
            return new Rectangle(
                (int)PixelPosition.X - boxSize / 2,
                (int)PixelPosition.Y - boxSize / 2,
                boxSize,
                boxSize
            );
        }

        public bool CheckCollision(Rectangle vehicleBounds)
        {
            return GetBounds().Intersects(vehicleBounds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsAlive) return;
            
            // Choose sprite based on whether moving
            Texture2D currentSprite = _isMoving ? _hopSprite : _idleSprite;
            
            // Draw centered on pixel position
            Vector2 origin = new Vector2(currentSprite.Width / 2, currentSprite.Height / 2);
            
            spriteBatch.Draw(
                currentSprite,
                PixelPosition,
                null,
                Color.White,
                0f,
                origin,
                0.3f, // Scale down the duck to fit grid
                SpriteEffects.None,
                0f
            );
        }

        public void ResetPosition()
        {
            GridPosition = new Vector2(5, 10); // Starting position
            PixelPosition = GridToPixel(GridPosition);
        }

        public void Reset()
        {
            ResetPosition();
            IsAlive = true;
            _isMoving = false;
            _moveTimer = 0f;
            _animationTimer = 0f;
        }
    }
}