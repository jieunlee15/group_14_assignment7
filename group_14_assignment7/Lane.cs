using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace group_14_assignment7
{
    public enum LaneType
    {
        Grass,
        Road,
        River
    }

    public enum Direction
    {
        Left,
        Right
    }

    public class Lane
    {
        public int LaneIndex;
        public LaneType Type;
        public List<Vehicle> Vehicles;
        public Direction VehicleDirection;
        public float VehicleSpeed;
        public Texture2D LaneTexture;
        private float _spawnTimer;
        private float _spawnInterval;

        private Texture2D _vehicleTexture;
        private Player _player;
        private int _screenWidth;
        private int _laneY;
        private const int TileSize = 64;

        public Lane(
            int laneIndex,
            LaneType type,
            Direction direction,
            float speed,
            float spawnInterval,
            Texture2D laneTexture,
            Texture2D vehicleTexture,
            Player player,
            int screenWidth)
        {
            LaneIndex = laneIndex;
            Type = type;
            VehicleDirection = direction;
            VehicleSpeed = speed;
            _spawnInterval = spawnInterval;
            LaneTexture = laneTexture;
            _vehicleTexture = vehicleTexture;
            _player = player;
            _screenWidth = screenWidth;
            _laneY = laneIndex * TileSize;
            Random rng = new Random(laneIndex * 7 + 13);
            _spawnTimer = (float)(rng.NextDouble() * spawnInterval);
            Vehicles = new List<Vehicle>();
        }

        public void Update(GameTime gameTime)
        {
            if (Type == LaneType.Grass)
                return;

            foreach (Vehicle v in Vehicles)
                v.Move();


            if (Vehicles.Count < 1)
            {
                _spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_spawnTimer >= _spawnInterval)
                {
                    _spawnTimer = 0f;
                    SpawnVehicle();
                }
            }
        }

        private void SpawnVehicle()
        {
            float startX = VehicleDirection == Direction.Left ? _screenWidth + 10 : -70;
            float velocityX = VehicleDirection == Direction.Left ? -VehicleSpeed : VehicleSpeed;
            Vector2 spawnPos = new Vector2(startX, _laneY);
            Vector2 velocity = new Vector2(velocityX, 0);

            Vehicles.Add(new Vehicle(
                _vehicleTexture,
                spawnPos,
                velocity,
                _player,
                new Vector2(_screenWidth, 0),
                VehicleDirection == Direction.Left,
                0.2f
            ));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (LaneTexture != null)
            {
                for (int x = 0; x < _screenWidth; x += TileSize)
                    spriteBatch.Draw(LaneTexture, new Vector2(x, _laneY), Color.White);
            }

            foreach (Vehicle v in Vehicles)
                v.Draw(spriteBatch);
        }

        public List<Vehicle> GetActiveVehicles()
        {
            return Vehicles;
        }
    }
}