using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroid_Belt_Assault
{
    class PlanetManager
    {
        private List<Sprite> planets = new List<Sprite>();
        private List<Rectangle> planetRects = new List<Rectangle>();
        private int screenWidth = 800;
        private int screenHeight = 600;
        private Random rand;

        public static int seed = 0;

        public PlanetManager(
            int screenWidth,
            int screenHeight,
            Vector2 starVelocity,
            Texture2D texture)
        {
            seed += System.Environment.TickCount;
            rand = new Random(seed);

            planetRects.Add(new Rectangle(88, 279, 173, 156));
            planetRects.Add(new Rectangle(341, 280, 117, 110));
            planetRects.Add(new Rectangle(69, 43, 165, 162));

            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;

            foreach (Rectangle r in planetRects)
            {
                planets.Add(new Sprite(
                    new Vector2(rand.Next(0, screenWidth),
                        -200),
                    texture,
                    r,
                    Vector2.Zero));
            }

            planets[rand.Next(0, planets.Count)].Velocity = new Vector2(0, 50);

        }

        public void Update(GameTime gameTime)
        {
            foreach (Sprite star in planets)
            {
                star.Update(gameTime);
                if (star.Location.Y > screenHeight)
                {
                    
                    star.Location = new Vector2(
                        rand.Next(0, screenWidth), -200);
                    star.Velocity = Vector2.Zero;

                    planets[rand.Next(0, planets.Count)].Velocity = new Vector2(0, 50);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite star in planets)
            {
                star.Draw(spriteBatch);
            }
        }

    }

}
