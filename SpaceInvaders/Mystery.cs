using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace SpaceInvaders
{
    public class Mystery
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 32;
        public int Height { get; set; } = 22;
        public int Speed { get; set; } = 2;
        public Dictionary<string, Rectangle> Ships { get; set; }
        public int screenX { get; set; }
        private static Random rnd = new();
        private bool shipActive;
        public static bool IsSameLevel = false;
        private string activeShipDirection;

        public Mystery(int screenX)
        {
            this.screenX = screenX;
            Ships = new Dictionary<string, Rectangle>
            {
                {"left", new Rectangle(-Width, 20, Width, Height)},
                {"right", new Rectangle(screenX, 20, Width, Height)}
            };
            shipActive = false;
            activeShipDirection = string.Empty;
        }

        private void GenerateShip(Song song)
        {
            if (!shipActive && rnd.Next(0, 25) == 1 && !IsSameLevel)
            {
                shipActive = true;
                activeShipDirection = rnd.Next(0, 2) == 0 ? "left" : "right";
                PlaySongEffect(song);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture, GameTime gametime, int screenY, EnemyList enemies, Song song)
        {
            if (gametime.TotalGameTime.TotalMilliseconds % 1000 < 16)
            {
                GenerateShip(song);
            }

            if (shipActive && !Player.GameOver(enemies, screenY))
            {
                spriteBatch.Draw(texture, Ships[activeShipDirection], Color.White);
                IsSameLevel = true;
            }
        }

        public void Move()
        {
            if (shipActive)
            {
                if (activeShipDirection == "left")
                {
                    
                    Ships[activeShipDirection] = new Rectangle(Ships[activeShipDirection].X + Speed, Ships[activeShipDirection].Y, Width, Height);
                }
                else
                {
                    Ships[activeShipDirection] = new Rectangle(Ships[activeShipDirection].X - Speed, Ships[activeShipDirection].Y, Width, Height);
                }
            }
        }

        public void PlaySongEffect(Song song)
        {
            if (shipActive)
            {
                MediaPlayer.Play(song);
                MediaPlayer.Volume = 0.1f;
            }
           

        }

        public bool IsAtEdge(int screenWidth)
        {
            if (activeShipDirection == "left" && Ships[activeShipDirection].X > screenWidth + Width)
            {
                return true;
            }
            if (activeShipDirection == "right" && Ships[activeShipDirection].X < -Width)
            {
                return true;
            }
            return false;
        }

        public void Eliminate(int screenWidth)
        {
            if (IsAtEdge(screenWidth))
            {
                shipActive = false;
                Ships["left"] = new Rectangle(-Width, 20, Width, Height);
                Ships["right"] = new Rectangle(screenX, 20, Width, Height);
                activeShipDirection = string.Empty;
            }
        }

        public void GetHit(Player player)
        {
            if (!shipActive) return;

            if (player.bullets.RemoveAll(b => b.Intersects(Ships[activeShipDirection])) > 0)
            {
                Player.Score += 50;
                shipActive = false;
                Ships["left"] = new Rectangle(-Width, 20, Width, Height);
                Ships["right"] = new Rectangle(screenX, 20, Width, Height);
                activeShipDirection = string.Empty;
            }
        }
    }
}