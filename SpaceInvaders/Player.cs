using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    public class Player
    {
        public int x;
        public int y;
        public static int Score = 0;
        public static int MaxScore = 0;
        public int level = 0;
        public int sizeX = 5;
        public int sizeY = 5;
        public static int Hearts = 3;
        public static int Lifes = 3;
        public int playerSizeX = 24;
        public int playerSizeY = 24;
        public int bulletSizeX = 4;
        public int bulletSizeY = 15;
        public bool Fired = false;
        public int bulletX;
        public int speed = 5;
        public int bulletY;
        public List<Rectangle> bullets = new List<Rectangle>();

        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
        }


        public void Move(int screenWidth, int screenHeight, EnemyList enemies)
        {
            if (! Player.GameOver(enemies, screenHeight))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                {
                    x -= speed;
                    if (x < 0)
                    {
                        x = 0;
                    }
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                {
                    x += speed;
                    if (x + playerSizeX > screenWidth)
                    {
                        x = screenWidth - playerSizeX;
                    }
                }
            }
            
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int ScreenY, EnemyList enemies)
        {
            if (! GameOver(enemies, ScreenY))
            {
                spriteBatch.Draw(texture, new Rectangle(x, y, playerSizeX, playerSizeY), Color.DarkGray);
            }
            
        }

        public void Fire(GameTime gameTime, SoundEffectInstance songInstance, SoundEffect song, int ScreenY, EnemyList enemies)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && !Fired && bullets.Count < 1 && !GameOver(enemies, ScreenY))
            {
                songInstance = song.CreateInstance();
                songInstance.Volume = 0.02f;
                songInstance.Play();
                Fired = true;
                bulletY = y - bulletSizeY;
                bulletX = x + (playerSizeX - bulletSizeX) / 2;
                AddBullet();
            }

            if (gameTime.TotalGameTime.TotalMilliseconds % 500 < 16)
            {
                Fired = false;
            }
            
        }

        public static bool GameOver(EnemyList enemies, int ScreenY)
        {
            for(int i = 0; i < enemies.enemies.Count; i++)
            {
                if (enemies.enemies[i].y >= ScreenY - ScreenY / 4)
                {
                    return true;
                }
            }
            return Hearts <= 0 && Lifes <= 0;
        }

        public void Reset(int ScreenX, int ScreenY)
        {
            Mystery.IsSameLevel = false;
            Lifes = 3;
            Hearts = 3;
            bullets.Clear();
            Fired = false;
            Score = 0;
            x = ScreenX / 2;
            y = ScreenY - ScreenY / 5;
        }

        public void AddBullet()
        {
            bullets.Add(new Rectangle(bulletX, bulletY, bulletSizeX, bulletSizeY));
        }

        public void DrawBullet(SpriteBatch spriteBatch, Texture2D texture)
        {
            foreach(var bullet in bullets)
            {
               spriteBatch.Draw(texture, bullet, Color.White);
            }
                
        }

        public void MoveBullet()
        {
               for (int i = bullets.Count - 1; i >= 0; i--)
               {
                    Rectangle bullet = bullets[i];
                    bullet.Y -= 10;
                    if (bullet.Y < 0)
                    {
                        bullets.RemoveAt(i);
                    }
                    else
                    {
                        bullets[i] = bullet;
                    }
               }
        }
    }
}