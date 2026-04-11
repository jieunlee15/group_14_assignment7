using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace group_14_assignment7;

public class Vehicle
{
    private Vector2 position;
    private Vector2 velocity;
    private Texture2D texture;
    private Rectangle collider;
    private Vector2 windowBounds;
    
    private bool moveLeft = false;
    
    private Player player;

    public Vehicle(Texture2D texture, Vector2 position, Vector2 velocity, Player player, Vector2 windowBounds, bool moveLeft)
    {
        this.texture = texture;
        this.position = position;
        this.velocity = velocity;
        this.player = player;
        this.windowBounds = windowBounds;
        this.moveLeft = moveLeft;
        this.collider = new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
    }

    public void Move()
    {
        position += velocity;

        CheckOffScreen();
        
        collider.X = (int)position.X;
        collider.Y = (int)position.Y;
    }

    void CheckOffScreen()
    {
        if (moveLeft)
        {
            if (position.X < -texture.Width / 2)
            {
                position.X = windowBounds.X + texture.Width / 2;
            }
        }
        else
        {
            if (position.X > windowBounds.X + texture.Width / 2)
            {
                position.X = -texture.Width / 2;
            }
        }
    }

    public Rectangle GetCollider()
    {
        return collider;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
}