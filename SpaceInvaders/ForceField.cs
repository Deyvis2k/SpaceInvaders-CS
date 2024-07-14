using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceInvaders
{
    public class ForceField
    {
        public int sizeX = 8;
        public int sizeY = 8;
        public IDictionary<int, FieldData> fields = new Dictionary<int, FieldData>();
        private int nextId = 0;

        public class FieldData
        {
            public int[,] Field { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public int AddField(int x, int y)
        {
            var newField = new int[5, 9]
            {
                { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 0, 0, 0, 1, 1, 1 }
            };

            var fieldData = new FieldData { Field = newField, X = x, Y = y};
            int fieldId = nextId++;
            fields[fieldId] = fieldData;
            return fieldId;
        }

        public void DrawFields(SpriteBatch spriteBatch, Texture2D texture, int screenY, EnemyList enemies)
        {
            if (!Player.GameOver(enemies, screenY))
            {
                foreach (var field in fields.Values)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (field.Field[i, j] != 0)
                            {
                                spriteBatch.Draw(texture, new Rectangle(field.X + j * sizeX, field.Y + i * sizeY, sizeX, sizeY), Color.MediumSeaGreen);
                            }
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            foreach (var field in fields.Values)
            {
                field.Field = new int[5, 9]
                {
                    { 0, 0, 0, 1, 1, 1, 0, 0, 0 },
                    { 0, 1, 1, 1, 1, 1, 1, 1, 0 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                    { 1, 1, 1, 0, 0, 0, 1, 1, 1 }
                };
            }
        }


        public void GetHit(Player player)
        {
            foreach (var field in fields.Values)
            {
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (field.Field[i, j] != 0)
                        {
                            for (int k = player.bullets.Count - 1; k >= 0; k--)
                            {
                                if (player.bullets[k].Intersects(new Rectangle(field.X + j * sizeX, field.Y + i * sizeY, sizeX, sizeY)))
                                {
                                    field.Field[i, j] = 0;
                                    player.bullets.RemoveAt(k);
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