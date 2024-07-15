using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SpaceInvaders
{
    public class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int  Width { get; set; } = 5;
        public int Height { get; set; } = 15;
        public int Speed { get; set; } = 10;

        public Bullet(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move()
        {
            Y += Speed;
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(X, Y, Width, Height);
        }
    }

    public class Enemy
    {
        private int sizeX = 20;
        private int sizeY = 20;
        private Texture2D texture;
        public int x { get; set; }
        public int y { get; set; }
        public int Speed { get; set; }
        public static int TotalEliminated { get; set; } = 0;
        public List<Bullet> bullets = new List<Bullet>();
        private static Random rnd = new Random();

        public Enemy(int x = 0, int y = 0, Texture2D texture = null, int Speed = 1)
        {
            this.texture = texture;
            this.x = x;
            this.y = y;
            this.Speed = Speed;
        }

        public void Draw(SpriteBatch spriteBatch, int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                spriteBatch.Draw(texture, new Rectangle(x, y, sizeX, sizeY), Color.White);
            }
        }

        public void IncreaseSpeed()
        {
            Speed += 1;
        }

        public void Move(int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                x += Speed;
            }
        }
            
        public bool IsAtEdge(int screenWidth)
        {
            return x + sizeX > screenWidth || x < 0;
        }

        public bool IsHitted(Rectangle bullet)
        {
            Rectangle enemy = new Rectangle(x, y, sizeX, sizeY);
            return enemy.Intersects(bullet);
        }

        public void MoveDown()
        {
            Speed = - Speed;
            y += 15;
        }

        public void Fire(GameTime gameTime, int screenY, EnemyList enemies)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds % 1000 < 16 && rnd.Next(0, 20) == 1 && !Player.GameOver(enemies, screenY))
            {
                int bulletX = x + (sizeX - 10) / 2;
                int bulletY = y + sizeY;
                bullets.Add(new Bullet(bulletX, bulletY));
            }
        }

        public void MoveBullets(int screenY)
        {
            bullets.RemoveAll(b => { b.Move(); return b.Y >= screenY - screenY / 8; });
        }

        public void DrawBullets(SpriteBatch spriteBatch, Texture2D bulletTexture)
        {
            foreach (var bullet in bullets)
            {
                spriteBatch.Draw(bulletTexture, bullet.GetBounds(), Color.Red);
            }
        }
    }

    public class EnemyList
    {
        private int addX = 20;
        private int addY = 30;
        public int level = 0;
        public List<Enemy> enemies = new List<Enemy>();

        public void AddEnemy(Texture2D[] texture)
        {
            int rowsPerTexture = 5 / texture.Length;
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 12 + level; j++)
                {
                    int textureIndex = i / rowsPerTexture;
                    textureIndex = Math.Min(textureIndex, texture.Length - 1);
                    Texture2D enemyTexture = texture[textureIndex];
                    enemies.Add(new Enemy(j * 35 + addX, i * 35 + addY, enemyTexture));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int screenY)
        {
            foreach (var enemy in enemies)
            {
                enemy.Draw(spriteBatch, screenY, this);
            }
        }

        public void Move(int screenWidth, int screenY)
        {
            bool anyAtEdge = false;
            foreach (var enemy in enemies)
            {
                enemy.Move(screenY, this);
                if (enemy.IsAtEdge(screenWidth))
                {
                    anyAtEdge = true;
                }
            }
            if (anyAtEdge)
            {
                foreach (var enemy in enemies)
                {
                    enemy.MoveDown();
                }
            }
        }

        public void GetHitted(Player player)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                var enemy = enemies[i];
                for (int j = player.bullets.Count - 1; j >= 0; j--)
                {
                    var bullet = player.bullets[j];
                    if (enemy.IsHitted(bullet))
                    {
                        enemies.RemoveAt(i);
                        player.bullets.RemoveAt(j);
                        Player.Score += 10;
                        break;
                    }
                }
            }
        }

        public bool IsAllDead()
        {
            return enemies.Count == 0;
        }

        public void Increment(Texture2D[] texture, Player player)
        {
            if (IsAllDead())
            {

                enemies.Clear();
                player.level++;
                level++;
                AddEnemy(texture);
                Enemy.TotalEliminated = 0;
                Mystery.IsSameLevel = false;
                IncreaseSpeed();
            }
        }

        public void Fire(GameTime gameTime, int screenY)
        {
            foreach (var enemy in enemies)
            {
                enemy.Fire(gameTime, screenY, this);
            }
        }

        public void MoveBullets(int screenY)
        {
            foreach (var enemy in enemies)
            {
                enemy.MoveBullets(screenY);
            }
        }

        public void DrawBullets(SpriteBatch spriteBatch, Texture2D bulletTexture, int screenY)
        {
            if (!Player.GameOver(this, screenY))
            {
                foreach (var enemy in enemies)
                {
                    enemy.DrawBullets(spriteBatch, bulletTexture);
                }
            }
        }

        public void Reset(Texture2D[] texture)
        {
            if(IsAllDead())
            {
                enemies.RemoveAt(0);
                Enemy.TotalEliminated = 0;
                level = 0;
                enemies.Clear();
                AddEnemy(texture);
            }
        }

        public void GameOverReset(Texture2D[] texture)
        {
            enemies.RemoveAt(0);
            Enemy.TotalEliminated = 0;
            level = 0;
            enemies.Clear();
            AddEnemy(texture);
        }

        public void HitPlayer(Player player)
        {
            foreach (var enemy in enemies)
            {
                for (int i = enemy.bullets.Count - 1; i >= 0; i--)
                {
                    if (enemy.bullets[i].GetBounds().Intersects(new Rectangle(player.x, player.y, player.playerSizeX, player.playerSizeY)))
                    {
                        Player.Hearts--;
                        enemy.bullets.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void IncreaseSpeed()
        {
            foreach (var enemy in enemies)
            {
                enemy.IncreaseSpeed();
            }
        }

        
        public void HitField(ForceField field)
        {
            foreach (var fieldData in field.fields.Values)
            {
                foreach (var enemy in enemies)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (fieldData.Field[i, j] != 0)
                            {
                                for (int k = enemy.bullets.Count - 1; k >= 0; k--)
                                {
                                    if (enemy.bullets[k].GetBounds().Intersects(new Rectangle(fieldData.X + j * 7, fieldData.Y + i * 7, 7, 7)))
                                    {
                                        fieldData.Field[i, j] = 0;
                                        enemy.bullets.RemoveAt(k);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}