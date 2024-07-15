using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
{
    public class Gui
    {
        public int x { get; set; }
        public int y { get; set; }

        public Gui(int x, int y)
        {
            this.x = x;
            this.y = y;
        }


        public void Draw(SpriteBatch spriteBatch, Texture2D texture, int screenWidth, int screenHeight, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenHeight))
            {
                spriteBatch.Draw(texture, new Rectangle(x, y, screenWidth, 5), Color.DarkGoldenrod);
            }
            
        }

        public void DrawHearts(SpriteBatch spriteBatch, Texture2D texture, int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                for (int i = 0; i < Player.Hearts; i++)
                {
                    spriteBatch.Draw(texture, new Rectangle(40 + x + i * 45, y + 15, 32, 32), Color.DarkGray);
                }
            }
            
        }

        public static void DrawGameOver(SpriteBatch spriteBatch, SpriteFont font, int screenX, int screenY, EnemyList enemies)
        {
            if (Player.GameOver(enemies, screenY))
            {
                spriteBatch.DrawString(font, "GAME OVER", new Vector2(screenX / 2 - 100, screenY / 2), Color.Red);
                spriteBatch.DrawString(font, "Press ENTER to restart", new Vector2(screenX / 2 - 170, screenY - screenY / 3 - 60), Color.Red);
            }
        }

        public static void DrawScores(SpriteBatch _spriteBatch, SpriteFont comic, int screenX, int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                _spriteBatch.DrawString(comic, $"Score: {Player.Score}", new Vector2(20, 8), Color.DarkGoldenrod);
                _spriteBatch.DrawString(comic, $"Max-Score: {Player.MaxScore}", new Vector2(screenX - screenX / 3 + 35, 8), Color.DarkGoldenrod);
            }   
        }

        public static void DrawPlayerInformation(SpriteBatch _spriteBatch, SpriteFont comic, int screenX, int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                string playerHearts = Player.Lifes >= 0 ? Player.Lifes.ToString() : "0";
                _spriteBatch.DrawString(comic, playerHearts, new Vector2(15, screenY - screenY / 12), Color.DarkGoldenrod);
                _spriteBatch.DrawString(comic, $"Level: {enemies.level + 1}", new Vector2(screenX - 110, screenY - screenY / 11), Color.DarkGoldenrod);
            }
        }
    }
}
