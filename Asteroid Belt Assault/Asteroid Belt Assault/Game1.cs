using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroid_Belt_Assault
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameStates { TitleScreen, Playing, BossBattle, PlayerDead, GameOver, YouWin };
        GameStates gameState = GameStates.TitleScreen;
        Texture2D titleScreen;
        Texture2D spriteSheet;
        Texture2D planetSheet;
        Texture2D Guydish;
        Texture2D Winner;
        Texture2D Boss;

        List<StarField> starFields;
        AsteroidManager asteroidManager;
        PlayerManager playerManager;
        EnemyManager enemyManager;
        ExplosionManager explosionManager;
        PlanetManager planetManager;
        BossManager bossManager;

        CollisionManager collisionManager;

        SpriteFont pericles14;

        

        private float playerDeathDelayTime = 10f;
        private float playerDeathTimer = 0f;
        private float titleScreenTimer = 0f;
        private float titleScreenDelayTime = 1f;

        private int playerStartingLives = 3;
        private int playerStartingHealth = 100;
        private Vector2 playerStartLocation = new Vector2(390, 550);
        private Vector2 scoreLocation = new Vector2(20, 10);
        private Vector2 livesLocation = new Vector2(20, 25);
        private Vector2 healthLocation = new Vector2(20, 40);


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            titleScreen = Content.Load<Texture2D>(@"Textures\TitleScreen");
            spriteSheet = Content.Load<Texture2D>(@"Textures\spriteSheet");
            planetSheet = Content.Load<Texture2D>(@"Textures\Planets");
            Guydish = Content.Load<Texture2D>(@"Textures\Guydish");
            Winner = Content.Load<Texture2D>(@"Textures\Winner");
            Boss = Content.Load<Texture2D>(@"Textures\Boss");

            planetManager = new PlanetManager(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                new Vector2(0, 40f),
                planetSheet);


            starFields = new List<StarField>();

            starFields.Add(new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                50,
                new Vector2(0, 40f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2)));

            starFields.Add(new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                150,
                new Vector2(0, 30f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2)));

            starFields.Add(new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                150,
                new Vector2(0, 15f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2)));

            starFields.Add(new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                50,
                new Vector2(0, 5f),
                spriteSheet,
                new Rectangle(0, 450, 2, 2)));

            asteroidManager = new AsteroidManager(
                5,
                spriteSheet,
                new Rectangle(0, 0, 50, 50),
                20,
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height);

            playerManager = new PlayerManager(
                spriteSheet,    
                new Rectangle(7, 146, 62, 62),    
                1,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));

            enemyManager = new EnemyManager(
                spriteSheet,
                new Rectangle(0, 200, 50, 50),
                6,
                playerManager,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));

            bossManager = new BossManager(
                spriteSheet,
                new Rectangle(0, 200, 50, 50),
                6,
                playerManager,
                new Rectangle(
                    0,
                    0,
                    this.Window.ClientBounds.Width,
                    this.Window.ClientBounds.Height));

            explosionManager = new ExplosionManager(
                spriteSheet,
                new Rectangle(0, 100, 50, 50),
                3,
                new Rectangle(0, 450, 2, 2));

            collisionManager = new CollisionManager(
                asteroidManager,
                playerManager,
                enemyManager,
                explosionManager);

            SoundManager.Initialize(Content);

            pericles14 = Content.Load<SpriteFont>(@"Fonts\Pericles14");


            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void resetGame()
        {
            playerManager.playerSprite.Location = playerStartLocation;
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                asteroid.Location = new Vector2(-500, -500);
            }
            playerManager.HealthRemaining = playerStartingHealth;
            enemyManager.Enemies.Clear();
            enemyManager.Active = true;
            playerManager.PlayerShotManager.Shots.Clear();
            enemyManager.EnemyShotManager.Shots.Clear();
            playerManager.Destroyed = false;
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    titleScreenTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (titleScreenTimer >= titleScreenDelayTime)
                    {
                        if ((Keyboard.GetState().IsKeyDown(Keys.Space)) ||
                            (GamePad.GetState(PlayerIndex.One).Buttons.A ==
                            ButtonState.Pressed))
                        {
                            playerManager.LivesRemaining = playerStartingLives;
                            playerManager.PlayerScore = 0;
                            resetGame();
                            gameState = GameStates.Playing;
                        }
                    }
                    break;

                case GameStates.Playing:

                    for (int i = 0; i < starFields.Count; i++)
                        starFields[i].Update(gameTime);

                    asteroidManager.Update(gameTime);
                    playerManager.Update(gameTime);

                    if (gameState != GameStates.BossBattle)
                        enemyManager.Update(gameTime);

                    explosionManager.Update(gameTime);
                    collisionManager.CheckCollisions();
                    planetManager.Update(gameTime);

                    if (playerManager.Destroyed)
                    {
                        playerDeathTimer = 0f;
                        enemyManager.Active = false;
                        playerManager.LivesRemaining--;
                        if (playerManager.LivesRemaining < 0)
                        {
                            gameState = GameStates.GameOver;
                            spriteBatch.Draw(Guydish,
                        new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
                        }
                        else
                        {
                            gameState = GameStates.PlayerDead;
                        }
                    }

                    if (playerManager.PlayerScore >= 1000)
                    {
                        gameState = GameStates.BossBattle;
                        asteroidManager.Clear();
                        enemyManager.Enemies.Clear();

                    }

                    break;

                    

                case GameStates.PlayerDead:
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;


                    for (int i = 0; i < starFields.Count; i++)
                        starFields[i].Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    planetManager.Update(gameTime);

                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        resetGame();
                        gameState = GameStates.Playing;
                    }
                    break;

                case GameStates.GameOver:
                    playerDeathTimer +=
                        (float)gameTime.ElapsedGameTime.TotalSeconds;

                    for (int i = 0; i < starFields.Count; i++)
                        starFields[i].Update(gameTime);
                    asteroidManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    playerManager.PlayerShotManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    planetManager.Update(gameTime);
                    if (playerDeathTimer >= playerDeathDelayTime)
                    {
                        gameState = GameStates.TitleScreen;
                    }
                    break;


                case GameStates.BossBattle:
                    for (int i = 0; i < starFields.Count; i++)
                        starFields[i].Update(gameTime);
                    playerManager.Update(gameTime);
                    enemyManager.Update(gameTime);
                    explosionManager.Update(gameTime);
                    collisionManager.CheckCollisions();
                    planetManager.Update(gameTime);

                    break;

            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (gameState == GameStates.TitleScreen)
            {
                spriteBatch.Draw(titleScreen,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.PlayerDead) ||
                (gameState == GameStates.GameOver) ||
                gameState == GameStates.BossBattle)
            {
                for (int i = 0; i < starFields.Count; i++)
                    starFields[i].Draw(spriteBatch);
                planetManager.Draw(spriteBatch);

                if (gameState != GameStates.BossBattle)
                {
                    asteroidManager.Draw(spriteBatch);
                    enemyManager.Draw(spriteBatch);
                }
                else
                {
                    bossManager.Draw(spriteBatch);
                }

                playerManager.Draw(spriteBatch);

                explosionManager.Draw(spriteBatch);

                spriteBatch.DrawString(
                    pericles14,
                    "Score: " + playerManager.PlayerScore.ToString(),
                    scoreLocation,
                    Color.White);

                spriteBatch.DrawString(
                    pericles14,
                    "Health: " + playerManager.HealthRemaining.ToString(),
                    healthLocation,
                    Color.White);

                if (playerManager.LivesRemaining >= 0)
                {
                    spriteBatch.DrawString(
                        pericles14,
                        "Ships Remaining: " + 
                            playerManager.LivesRemaining.ToString(),
                        livesLocation,
                        Color.White);
                }
                if (playerManager.HealthRemaining <= 0)
                {
                    playerManager.Destroyed = true;
                }
            }
            if ((playerManager.Destroyed == true))
            {
                spriteBatch.DrawString(
                    pericles14,
                    "Y O U  D I E D !",
                    new Vector2(
                        this.Window.ClientBounds.Width / 2 -
                          pericles14.MeasureString("Y O U  D I E D !").X / 2,
                        50),
                    Color.White);

                
            }
            if ((gameState == GameStates.GameOver))
            {
                


                spriteBatch.DrawString(
                    pericles14,
                    "G A M E  O V E R !",
                    new Vector2(
                        this.Window.ClientBounds.Width / 2 -
                          pericles14.MeasureString("G A M E  O V E R !").X / 2,
                        50),
                    Color.White);
            }
            if (gameState == GameStates.YouWin)
            {
                spriteBatch.Draw(Winner,
                    new Rectangle(0, 0, this.Window.ClientBounds.Width,
                        this.Window.ClientBounds.Height),
                        Color.White);
            }


            spriteBatch.End();

            base.Draw(gameTime);
        }

    }
}
