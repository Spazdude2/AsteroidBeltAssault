﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Asteroid_Belt_Assault
{
    class CollisionManager
    {
        private AsteroidManager asteroidManager;
        private PlayerManager playerManager;
        private EnemyManager enemyManager;
        private ExplosionManager explosionManager;
        private Vector2 offScreen = new Vector2(-500, -500);
        private Vector2 shotToAsteroidImpact = new Vector2(0, -20);
        private int enemyPointValue = 100;
        private int shotHealthValue = -10;
        private int asteroidHealthValue = -25;

        public CollisionManager(
            AsteroidManager asteroidManager,
            PlayerManager playerManager,
            EnemyManager enemyManager,
            ExplosionManager explosionManager)
        {
            this.asteroidManager = asteroidManager;
            this.playerManager = playerManager;
            this.enemyManager = enemyManager;
            this.explosionManager = explosionManager;
        }

        private void checkShotToEnemyCollisions()
        {
            foreach (Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (shot.IsCircleColliding(
                        enemy.EnemySprite.Center,
                        enemy.EnemySprite.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        enemy.Destroyed = true;
                        playerManager.PlayerScore += enemyPointValue;
                        explosionManager.AddExplosion(
                            enemy.EnemySprite.Center,
                            enemy.EnemySprite.Velocity / 10);
                    }

                }
            }
        }

        private void checkShotToAsteroidCollisions()
        {
            foreach (Sprite shot in playerManager.PlayerShotManager.Shots)
            {
                foreach (Sprite asteroid in asteroidManager.Asteroids)
                {
                    if (shot.IsCircleColliding(
                        asteroid.Center,
                        asteroid.CollisionRadius))
                    {
                        shot.Location = offScreen;
                        asteroid.Velocity += shotToAsteroidImpact;
                    }
                }
            }
        }

        private void checkShotToPlayerCollisions()
        {
            foreach (Sprite shot in enemyManager.EnemyShotManager.Shots)
            {
                if (shot.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    shot.Location = offScreen;
                    playerManager.HealthRemaining += shotHealthValue;
                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        private void checkEnemyToPlayerCollisions()
        {
            foreach (Enemy enemy in enemyManager.Enemies)
            {
                if (enemy.EnemySprite.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    enemy.Destroyed = true;
                    explosionManager.AddExplosion(
                        enemy.EnemySprite.Center,
                        enemy.EnemySprite.Velocity / 10);

                    playerManager.HealthRemaining = 0;

                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        private void checkAsteroidToPlayerCollisions()
        {
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                if (asteroid.IsCircleColliding(
                    playerManager.playerSprite.Center,
                    playerManager.playerSprite.CollisionRadius))
                {
                    explosionManager.AddExplosion(
                        asteroid.Center,
                        asteroid.Velocity / 10);

                    asteroid.Location = offScreen;

                    playerManager.HealthRemaining += asteroidHealthValue;
                    explosionManager.AddExplosion(
                        playerManager.playerSprite.Center,
                        Vector2.Zero);
                }
            }
        }

        private void checkAsteroidToEnemyCollisions()
        {
            foreach (Sprite asteroid in asteroidManager.Asteroids)
            {
                foreach (Enemy enemy in enemyManager.Enemies)
                {
                    if (asteroid.IsCircleColliding(
                     enemy.EnemySprite.Center,
                     enemy.EnemySprite.CollisionRadius))
                    {
                        explosionManager.AddExplosion(
                            asteroid.Center,
                            asteroid.Velocity / 10);

                        asteroid.Location = offScreen;

                        enemy.Destroyed = true;
                        explosionManager.AddExplosion(
                            enemy.EnemySprite.Center,
                            Vector2.Zero);
                    }
                }
            }
        }

        public void CheckCollisions()
        {
            checkShotToEnemyCollisions();
            checkAsteroidToEnemyCollisions();
            checkShotToAsteroidCollisions();
            if (!playerManager.Destroyed)
            {
                checkShotToPlayerCollisions();
                checkEnemyToPlayerCollisions();
                checkAsteroidToPlayerCollisions();
            }
        }

    }
}
