#region Using Statements
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
#endregion

namespace SpaceShip_nw8
{
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }

    class GameScreen
    {
        #region Properties variables

        bool isPopUp = false;
        TimeSpan transitionOn_time = TimeSpan.Zero;
        TimeSpan transitionOff_time = TimeSpan.Zero;
        float transPos = 1;
        float transAlpha = 1;
        ScreenState scrSt = ScreenState.TransitionOn;
        bool isExiting = false;
        GestureType enableGestures = GestureType.None;

        bool otherScreenHasFocus;
        ScreenManager scrManager;
        PlayerIndex? controlPlayer;

        #endregion

        #region Properties

        public bool IsPopUp
        {
            get { return isPopUp; }
            protected set { isPopUp = value; }
        }

        public TimeSpan TransitionOnTime
        {
            get { return transitionOn_time; }
            protected set { transitionOn_time = value; }
        }

        public TimeSpan TransitionOffTime
        {
            get { return transitionOff_time; }
            protected set { transitionOff_time = value; }
        }

        public float TransitionPosition
        {
            get { return transPos; }
            protected set { transPos = value; }
        }

        public float TransitionAlpha
        {
            get { return transAlpha; }
            protected set { transAlpha = value; }
        }

        public ScreenState StateOfScreen
        {
            get { return scrSt; }
            protected set { scrSt = value; }
        }

        public bool IsExiting
        {
            get { return isExiting; }
            protected internal set { isExiting = value; }
        }

        public bool IsActive
        {
            get { return !otherScreenHasFocus && (scrSt == ScreenState.TransitionOn || scrSt == ScreenState.Active); }
        }

        public ScreenManager screenManager
        {
            get { return scrManager; }
            internal set { scrManager = value; }
        }

        public PlayerIndex? ControllingPlayer
        {
            get { return controlPlayer; }
            internal set { controlPlayer = value; }
        }

        public GestureType EnableGestures
        {
            get { return enableGestures; }
            protected set
            {
                enableGestures = value;

                if (StateOfScreen == ScreenState.Active)
                {
                    TouchPanel.EnabledGestures = value;
                }
            }
        }

        #endregion

        #region Initialization

       public virtual void LoadContent() { }
       public virtual void UnloadContent() { }

#endregion

        #region Update&Draw

               bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
               {
                   float transitionDelta;

                   if (time == TimeSpan.Zero)
                       transitionDelta = 1;
                   else
                       transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                                 time.TotalMilliseconds);

                   transPos += transitionDelta * direction;
                    if (((direction < 0) && (transPos <= 0)) ||
                        ((direction > 0) && (transPos >= 1)))
                    {
                        transPos = MathHelper.Clamp(transPos, 0, 1);
                        return false;
                    }

                    return true;
                }

               public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coverdByOtherScreen)
               {
                   this.otherScreenHasFocus = otherScreenHasFocus;

                   if (isExeting)
                   {
                       scrSt = ScreenState.TransitionOff;

                       if (!UpdateTransition(gameTime, transitionOff_time, 1))
                       {
                           screenManager.Remove(this);
                       }
                   }
                   else if (coverdByOtherScreen)
                       {
                           if (UpdateTransition(gameTime, transitionOff_time, 1))
                           {
                               scrSt = ScreenState.TransitionOff;
                           }
                           else
                           {
                               scrSt = ScreenState.Hidden;
                           }
                       }
                   else
                   {
                       if (UpdateTransition(gameTime, transitionOn_time, -1))
                       {
                           scrSt = ScreenState.TransitionOn;
                       }
                       else
                       {
                           scrSt = ScreenState.Active;
                       }
                   }
               }

               public virtual void HandleInput(InputState input) { }

               public virtual void Draw(GameTime gameTime) { }

        #endregion

        #region Public Methods

                public void ExitScreen()
                {
                    if (TransitionOffTime == TimeSpan.Zero)
                    {
                        ScreenManager.RemoveScreen(this);
                    }
                    else
                    {
                        isExiting = true;
                    }
                }

        #endregion
    }
}  

