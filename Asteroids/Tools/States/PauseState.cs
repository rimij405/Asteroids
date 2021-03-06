﻿/// PauseState.cs - Version 1.
/// Author: Ian Effendi
/// Date: 4.9.2017

#region Using statements.

// System using statements.
using System;
using System.Collections;
using System.Collections.Generic;

// MonoGame using statements.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Asteroids using statements.
using Asteroids.Attributes;
using Asteroids.Entities;

#endregion

namespace Asteroids.Tools.States
{
    /// <summary>
    /// The PauseState stores the functionality for States.Pause enum.
    /// </summary>
    public class PauseState : State
    {

        #region Constructors. // Sets this state's States enum flag to States.Pause.

        public PauseState(ColorSet set, float scale = 1.0f) : base(StateType.Pause, set, scale)
        {
            // Any special instructions for the pause menu should take place here.
        }

        public PauseState(Color draw, Color bg, float scale = 1.0f) : base(StateType.Pause, draw, bg, scale)
        {
            // Any special instructions for the pause menu should take place here.
        }

		#endregion

		#region Methods. // Methods that have been overriden from the parent class.

		#region Load methods. // Called to help faciliate the addition of entities to a given state.

		/// <summary>
		/// Facilitate the addition of buttons to a given state.
		/// </summary>
		/// <param name="button">Texture of the button.</param>
		public override void LoadButtons(Texture2D button, Padding screenPadding, Padding centerPadding, Vector2 bounds)
		{
			ShapeDrawer pen = GlobalManager.Pen;
			LoadButtons(new Button(Actions.Resume, pen, Positions.Center, new Vector2(0, centerPadding.GetY(-2)), bounds, button, "Resume"),
				new Button(Actions.Quit, pen, Positions.Center, new Vector2(0, centerPadding.GetY(-1)), null, button, "Quit to Windows"),
				new Button(Actions.Back, pen, Positions.BottomRight, screenPadding.Get(-1), bounds, button, "Back"));
		}

		#endregion

		#region Update methods. // Update calls for the Pause.

		/// <summary>
		/// Bind the debug key and escape key for the Pause.
		/// </summary>
		protected override void BindKeys()
        {
            Controls.Bind(Commands.Debug, Keys.D, ActionType.Released);
            Controls.Bind(Commands.Pause, Keys.P, ActionType.Released);
            Controls.Bind(Commands.Back, Keys.Escape, ActionType.Released);
        }

        /// <summary>
        /// Handles all key press inputs for a state.
        /// </summary>
        protected override void HandleInput()
        {
            if (!Controls.IsEmpty())
            {
                if (Controls.IsFired(Commands.Debug))
                {
                    this.Debug = !Debug;
                }

                if (Controls.IsFired(Commands.Pause) 
                    || GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed
                    || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    StateManager.TogglePause();
                }

                if (Controls.IsFired(Commands.Back))
                {
                    StateManager.ChangeState(StateType.Main);
                }
            }
        }

        /// <summary>
        /// Handles all button press inputs for a state.
        /// </summary>
        protected override void HandleGUIInput()
        {
            if (HasButtons)
            {
                if (IsActionFired(Actions.Resume))
                {
                    StateManager.TogglePause();
                }

                if (IsActionFired(Actions.Quit))
                {
                    StateManager.Quit();
                }
            }
        }
        
        /// <summary>
        /// Queue all the GUI messages that need to be printed.
        /// </summary>
        protected override void QueueGUIMessages()
        {
            string guiMessage = "";

            guiMessage += "Press the ESC key to return to the main menu." + "\n";
            guiMessage += "Press the P key to resume the game." + "\n";
            guiMessage += "Press the Q key to quit to the desktop." + "\n";
            guiMessage += "Throttle and Brake with W and S. Rotate with A and D." + "\n";
            guiMessage += "Turn with the Left and Right arrow keys.";

            // What to do during the pause functionality.  
            // Print message: Escape to quit. P to resume the game.
            Vector2 position = this.GetScreenCenter();
            Padding padding = new Padding(0, this.GetStringHeight(guiMessage));
            AddMessage(guiMessage, position, padding, this.DrawColor, 1, ShapeDrawer.CENTER_ALIGN);
        }

        #endregion
        
        #endregion

    }
}
