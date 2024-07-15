using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public Enemy enemy = new ();
        public EnemyList enemies = new ();
        public Gui gui;
        private Player player;
        public ForceField forcefield1 = new ();
        private Texture2D[] texture;
        private SoundEffectInstance bulletSoundInstance;
        private SoundEffect bulletSound;
        private Song MysterySound;



        public int screenX;
        public int screenY;
        Rectangle gameElement;
        Texture2D DefaultTexture;
        Texture2D playertexture;
        Texture2D bullet;
        Texture2D heartTexture;
        Texture2D MysteryTexture;

        private Mystery Mystery;
        public SpriteFont comic;

        public Game1()
        {
            _graphics = new(this);
            screenX = _graphics.PreferredBackBufferWidth;
            screenY = _graphics.PreferredBackBufferHeight;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        

        protected override void Initialize()
        {

            forcefield1.AddField(screenX / 6, screenY - screenY / 3);
            forcefield1.AddField(screenX / 2 - (screenX / 7), screenY - screenY / 3);
            forcefield1.AddField(screenX / 2 + (screenX / 16), screenY - screenY / 3);
            forcefield1.AddField(screenX - (screenX / 4) + (screenY / 28), screenY - screenY / 3);

            player = new Player(screenX / 2 , screenY - screenY / 5);

            gui = new(0, screenY - screenY / 8);


            Mystery = new(screenX);

            

            base.Initialize();
        }

        

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            texture = new Texture2D[]
            {
                Content.Load<Texture2D>("yellow"),
                 Content.Load<Texture2D>("green"),
                Content.Load<Texture2D>("green"),
                Content.Load<Texture2D>("red"),
                Content.Load<Texture2D>("red")
               
            };

            enemies.AddEnemy(texture);

            //--------------------------------------------//

            playertexture = Content.Load<Texture2D>("player");

            //--------------------------------------------//
            
            DefaultTexture = new Texture2D(GraphicsDevice, 1, 1);
            DefaultTexture.SetData(new[] { Color.White });

            //--------------------------------------------//

            bullet = new Texture2D(GraphicsDevice, 1, 1);
            bullet.SetData(new[] { Color.White });

            //--------------------------------------------//

            comic = Content.Load<SpriteFont>("Comic");
            heartTexture = Content.Load<Texture2D>("heart");
            MysteryTexture = Content.Load<Texture2D>("MysteryShips");

            bulletSound = Content.Load<SoundEffect>("lasersoundwav");
            bulletSoundInstance = bulletSound.CreateInstance();
            

            MysterySound = Content.Load<Song>("mysteryshipeffect");
            
        }

  
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Move(screenX, screenY, enemies);
            player.Fire(gameTime, bulletSoundInstance, bulletSound, screenY, enemies);
            player.MoveBullet();
            Player.MaxScore = Math.Max(Player.MaxScore, Player.Score);
            
            forcefield1.GetHit(player);
            enemies.Move(screenX, screenY);
            enemies.GetHitted(player);
            enemies.Increment(texture, player);
            enemies.HitField(forcefield1);
            enemies.HitPlayer(player);
            enemies.Fire(gameTime, screenY);
            enemies.MoveBullets(screenY);


            Mystery.Eliminate(screenX);
            Mystery.Move();
            Mystery.GetHit(player);



            if (Player.Hearts == 0)
            {
                Player.Lifes--;
                if (Player.Lifes < 0)
                {
                    Player.Hearts = 0;
                }
                else { Player.Hearts = 3; }
            }

            if (Player.GameOver(enemies, screenY))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    enemies.GameOverReset(texture);
                    player.Reset(screenX, screenY);
                    forcefield1.Reset();
                }

            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            for (int i = 0; i < 5; i++)
            {
                forcefield1.DrawFields(_spriteBatch, DefaultTexture, screenY, enemies);
            }
           
            player.Draw(_spriteBatch, playertexture, screenY, enemies);
            player.DrawBullet(_spriteBatch, bullet);
            enemies.Draw(_spriteBatch, screenY);
            enemies.DrawBullets(_spriteBatch, bullet, screenY);
            gui.Draw(_spriteBatch, DefaultTexture, screenX, screenY, enemies);
            gui.DrawHearts(_spriteBatch , playertexture, screenY, enemies);
            Gui.DrawPlayerInformation(_spriteBatch, comic, screenX, screenY , enemies);
            Gui.DrawScores(_spriteBatch, comic, screenX, screenY, enemies);
            Gui.DrawGameOver(_spriteBatch, comic, screenX, screenY, enemies);
            Mystery.Draw(_spriteBatch, MysteryTexture, gameTime, screenY, enemies, MysterySound);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
