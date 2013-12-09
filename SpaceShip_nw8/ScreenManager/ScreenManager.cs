#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
#endregion

namespace SpaceShip_nw8
{

    class ScreenManager : DrawableGameComponent
    {
        #region Field variables

        List<GameScreen>Screens = new List<GameScreen>();
        List<GameScreen>ScreensToUpdate = new List<GameScreen>();

        InputState input = new InputState();
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D _blankTexture;

        bool isInitialized, traceEnabled;

        #endregion

        #region Fields

        SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        SpriteFont SpriteFont
        {
            get { return font; }
        }

        public bool TraceEnabled
        {
            get { return traceEnabled; }
            set { traceEnabled = value; }
        }

        #endregion

        #region Initializing

        public ScreenManager(Game game)
            : base(game)
        {
            TouchPanel.EnabledGestures = GestureType.None;
        }

        public override void Initialized()
        {
            base.Initialize();
            isInitialized = true;
        }

        protected override void LoadContent()
        {
            ContentManager content = Game.Content;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = content.Load<SpriteFont>("menufont");
            _blankTexture = content.Load<Texture2D>("blank");

            foreach (var scr in Screens)
            {
                scr.LoadContent();
            }
        }

        protected override void UnloadContent()
        {
            foreach (var scr in Screens)
            {
                scr.UnloadContent();
            }
        }

        #endregion

        #region Update&Draw

        void TraceScreens()
        {
            List<string> screenNames = new List<string>();

            foreach (var scr in Screens)
                screenNames.Add(scr.GetType().Name);

            Debug.WriteLine(string.Join(", ", screenNames.ToArray()));
        }

        public override void Update(GameTime gameTime)
        {
            input.Update();
            ScreensToUpdate.Clear();

            foreach (var scr in Screens)
                ScreensToUpdate.Add(scr);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coverdByOtherScreen = false;

            while (ScreensToUpdate.Count > 0)
            {
                GameScreen screen = ScreensToUpdate[ScreensToUpdate.Count - 1];
                ScreensToUpdate.RemoveAt(ScreensToUpdate.Count - 1);

                screen.Update(gameTime, otherScreenHasFocus, coverdByOtherScreen);

                if (screen.StateOfScreen == ScreenState.TransitionOn || screen.StateOfScreen == ScreenState.Active)
                {
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(input);
                        coverdByOtherScreen = true;
                    }

                    if (!screen.IsPopUp) coverdByOtherScreen = true;
                }

                if (traceEnabled) TraceScreens;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var scr in Screens)
            {
                if (scr.StateOfScreen == ScreenState.Active) continue;

                scr.Draw(gameTime);
            }
        }

        #endregion

        #region public methods

        public void AddScreen(GameScreen scr, PlayerIndex? ControllingPlayer)
        {
            scr.ControllingPlayer = ControllingPlayer;
            scr.screenManager = this;
            scr.IsExiting = false;

            if (isInitialized) scr.LoadContent();

            Screens.Add(scr);

            TouchPanel.EnabledGestures = scr.EnableGestures;
        }

        public void RemoveScreen(GameScreen scr)
        {
            if (isInitialized) scr.UnloadContent();
            Screens.Remove(scr);
            ScreensToUpdate.Remove(scr);

            if (Screens.Count > 0) TouchPanel.EnabledGestures = Screens[Screens.Count - 1].EnableGestures;
        }

        public GameScreen[] GetScreens()
        {
            return Screens.ToArray();
        }

        public void FadeBackBufferToBlack(float alpha)
        {
            Viewport viewport = GraphicsDevice.Viewport;

            spriteBatch.Begin();

            spriteBatch.Draw(_blankTexture,
                             new Rectangle(0, 0, viewport.Width, viewport.Height),
                             Color.Black * alpha);

            spriteBatch.End();
        }

        #endregion
    }
}
