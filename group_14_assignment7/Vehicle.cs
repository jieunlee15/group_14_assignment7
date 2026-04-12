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

    private float scale;
    private Vector2 origin;
    
    private bool moveLeft = false;
    
    private Player player;

    public Vehicle(Texture2D texture, Vector2 position, Vector2 velocity, Player player, Vector2 windowBounds, bool moveLeft, float  scale)
    {
        this.texture = texture;
        this.position = position;
        this.velocity = velocity;
        this.player = player;
        this.windowBounds = windowBounds;
        this.moveLeft = moveLeft;
        this.scale = scale;
        this.collider = new Rectangle(
            (int)position.X, 
            (int)position.Y, 
            (int)(texture.Width * scale), 
            (int)(texture.Height * scale)
            );
        
        //origin = new Vector2(texture.Width / 2, texture.Height / 2);
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
        spriteBatch.Draw(
            texture, 
            position,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0
            );
    }
}