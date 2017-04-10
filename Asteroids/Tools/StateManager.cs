﻿/// StateManager.cs - Version 3
/// Author: Ian Effendi
/// Date: 3.26.2017

#region Using statements.

// System using statements.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Monogame using statements.
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Asteroid using statments.
using Asteroids.Entities;
using Asteroids.Attributes;
using Asteroids.Tools.States;

#endregion

namespace Asteroids.Tools
{

    #region Enums. // Contains definitions for the Actions and StateType enums.

    /// <summary>
    /// Possible actions to associate with a button press.
    /// </summary>
    public enum Actions
    {
        /// <summary>
        /// Start a new game.
        /// </summary>
        Start,

        /// <summary>
        /// Quit to windows.
        /// </summary>
        Quit,

        /// <summary>
        /// Return to the previous state.
        /// </summary>
        Back,

        /// <summary>
        /// Pause the active game.
        /// </summary>
        Pause,

        /// <summary>
        /// Resume the active game.
        /// </summary>
        Resume,
        
        /// <summary>
        /// Display the game's scores.
        /// </summary>
        Scores,

        /// <summary>
        /// Show the options menu.
        /// </summary>
        Options,

        /// <summary>
        /// Change the dimensions of the screen.
        /// </summary>
        Dimensions
    }

    /// <summary>
    /// Any of the possible states the game can be in.
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// The pause menu.
        /// </summary>
        Pause,

        /// <summary>
        /// The options screen.
        /// </summary>
        Options,

        /// <summary>
        /// The main menu.
        /// </summary>
        Main,

        /// <summary>
        /// The main game.
        /// </summary>
        Arena,

        /// <summary>
        /// The gameover and scores screen.
        /// </summary>
        Gameover,

        /// <summary>
        /// This state tells the game to quit.
        /// </summary>
        Quit,

		/// <summary>
		/// This is the null state.
		/// </summary>
		Null
    }

    #endregion

    public class StateManager
    {

        #region Fields. // Private data, lists, and hashtables for tracking the states.

        /// <summary>
        /// A collection of all the states.
        /// </summary>
        private static List<State> stateList;

        /// <summary>
        /// A hashtable of StateType enum modes and states.
        /// </summary>
        private static Dictionary<StateType, State> states;

        /// <summary>
        /// Tracks a list of messages as posted by the states to queue for the Message class.
        /// </summary>
        private static List<Message> messages;

        /// <summary>
        /// The game's current scale at which entities are drawn.
        /// </summary>
        private static float scale;

        // State tracking

        /// <summary>
        /// The current state's state enum value.
        /// </summary>
        private static StateType currentState;

        /// <summary>
        /// The previous state's state enum value.
        /// </summary>
        private static StateType previousState;

        #endregion

        #region Flags. // Flags are fields that indicate boolean off/on states for certain class traits.

        /// <summary>
        /// Determines if the class has been initialized or not.
        /// </summary>
        private static bool _initialized = false;

        #endregion

        #region Properties. // The properties provide access to the scale of the states.

        /// <summary>
        /// The scale of the game.
        /// </summary>
        public static float Scale
        {
            get { return scale; }
            set { SetScale(value); }
        }

        /// <summary>
        /// Returns the initialization status.
        /// </summary>
        public static bool Initialized
        {
            get { return _initialized; }
            private set { _initialized = value; }
        }

        /// <summary>
        /// Return the current state's draw color.
        /// </summary>
        public static Color DrawColor
        {
            get
            {
                return states[currentState].DrawColor;
            }
        }

        /// <summary>
        /// Return the current state's background color.
        /// </summary>
        public static Color Background
        {
            get
            {
                return states[currentState].BackgroundColor;
            }
        }

        #endregion

        #region Methods. // The methods to run for the state manager.

        #region Initialization. // Initialize the values inside of the state manager.

        /// <summary>
        /// Initializes and instantiates some of the static fields in the state manager.
        /// </summary>
        public static void Initialize()
        {
            // Create the basic states.
            stateList = new List<State>();
            states = new Dictionary<StateType, State>();
            messages = new List<Message>();

            // Set the initial state to the main state.
            currentState = StateType.Main;
            previousState = currentState;

            // Set initialization status to true.
            _initialized = true;
        }

        #endregion

        #region Service methods. // Methods that add functionality to the class or affect member variables.

        /// <summary>
        /// Sets the scale of all of the states.
        /// </summary>
        /// <param name="_scale">The scale to be set to.</param>
        public static void SetScale(float _scale = 1.0f)
        {
            scale = _scale;

            foreach (State s in stateList)
            {
                s.SetScale(_scale);
            }
        }

        /// <summary>
        /// Returns true if the state exists.
        /// </summary>
        /// <param name="type">The type of state to check for.</param>
        /// <returns>Returns true if it's been added.</returns>
        public static bool StateExists(StateType type)
        {
            return states.ContainsKey(type) && states[type] != null;
        }

        /// <summary>
        /// Add state to the states collection.
        /// </summary>
        /// <param name="key">State type.</param>
        /// <param name="draw">Draw color.</param>
        /// <param name="bg">Background color.</param>
        private static void AddState(StateType key, Color draw, Color bg)
        {
            switch (key)
            {
                case StateType.Main:
                    AddState(key, new MainMenuState(draw, bg));
                    break;
                case StateType.Pause:
                    AddState(key, new PauseState(draw, bg));
                    break;
                case StateType.Options:
                    AddState(key, new OptionsState(draw, bg));
                    break;
                case StateType.Arena:
                    AddState(key, new ArenaState(draw, bg));
                    break;
                case StateType.Gameover:
                    AddState(key, new ScoresState(draw, bg));
                    break;
                default:
                    AddState(key, new State(key, draw, bg));
                    break;
            }
        }

        /// <summary>
        /// Add the state to the manager.
        /// </summary>
        /// <param name="key">The state type.</param>
        /// <param name="state">The state being added.</param>
        private static void AddState(StateType key, State state)
        {
            if (states.ContainsKey(key))
            {
                states[key] = state;
            }
            else
            {
                states.Add(key, state);
            }

            stateList.Add(state);
            state.SetScale(scale);
        }

        /// <summary>
        /// Add an entity to the state associated with the StateType key.
        /// </summary>
        /// <param name="key">StateType key.</param>
        /// <param name="entity">Enitty being added.</param>
        public static void AddEntityToState(StateType key, Entity entity)
        {
            states[key].AddEntity(entity);
        }

        /// <summary>
        /// Add a button to the state associated with the StateType key.
        /// </summary>
        /// <param name="key">StateType key.</param>
        /// <param name="button">Button being added.</param>
        public static void AddButtonToState(StateType key, Button button)
        {
            states[key].AddButton(button);
        }

        /// <summary>
        /// Return the current state's StateType.
        /// </summary>
        /// <returns>Returns a StateType.</returns>
        public static StateType GetState()
        {
            return currentState;
        }

        #endregion

        #region State methods. // Methods that interact with the states.

        /// <summary>
        /// Change the current state and store the previous state.
        /// </summary>
        /// <param name="targetState">State to change to.</param>
        public static void ChangeState(StateType targetState)
        {
            // When changing states, set the previous state to the current state, 
            // Before setting target.

            previousState = currentState;
            currentState = targetState;

            states[previousState].Disable();
            states[currentState].Enable();
        }

        /// <summary>
        /// Rever to the previous state.
        /// </summary>
        public static void RevertState()
        {
            // Revert the states. The current state will be lost.

            states[currentState].Disable();

            currentState = previousState;
            states[currentState].Enable();
        }

        /// <summary>
        /// Toggle the pause menu.
        /// </summary>
        public static void TogglePause()
        {
            if (currentState != StateType.Pause)
            {
                ChangeState(StateType.Pause);
            }
            else
            {
                RevertState();
            }
        }

        /// <summary>
        /// Start the game by switching to the Arena state.
        /// </summary>
        public static void StartGame()
        {
            ChangeState(StateType.Arena);
            states[currentState].Start();
        }

        /// <summary>
        /// Go to the scores screen.
        /// </summary>
        public static void EndGame()
        {
            ChangeState(StateType.Gameover);
            previousState = StateType.Main;
        }

        /// <summary>
        /// Quit the game.
        /// </summary>
        public static void Quit()
        {
            ChangeState(StateType.Quit);
            previousState = StateType.Main;
        }
		
		/// <summary>
		/// Add a new message to the list of messages that will be queued.
		/// </summary>
		/// <param name="msg">Message string.</param>
		/// <param name="pos">Position of the message.</param>
		/// <param name="pad">Padding for the message.</param>
		/// <param name="draw">Color to draw the message in.</param>
		/// <param name="order">Order of priority.</param>
		/// <param name="alignment">Alignment of the string.</param>
		public static void AddMessage(string msg, Vector2 pos, Padding pad, Color draw, int order, int alignment)
		{
			messages.Add(new Message(msg, pos, pad, draw, order, alignment));
		}

		/// <summary>
		/// Add a message object to the list of messages that will be queued.
		/// </summary>
		/// <param name="msg">Message object.</param>
		public static void AddMessage(Message msg)
		{
			messages.Add(msg);
		}

		#endregion

		#region Reset/Start/Stop methods. // Reset, start, or stop the current state.

		/// <summary>
		/// Reset the current state.
		/// </summary>
		public static void Reset()
        {
            states[currentState].Reset();
        }

        /// <summary>
        /// Start the current state.
        /// </summary>
        public static void Start()
        {
            states[currentState].Start();
        }

        /// <summary>
        /// Stop the current state.
        /// </summary>
        public static void Stop()
        {
            states[currentState].Stop();
        }

        #endregion

        #region Update methods.

        /// <summary>
        /// Update the current state.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
			State curr = states[currentState];

			if (curr.StateType != StateType.Null && !curr.Empty)
			{
				curr.Update(gameTime);
			}
        }
		
        #endregion

        #region Draw methods.

        /// <summary>
        /// Draw the entities and buttons in the current state.
        /// </summary>
        public static void Draw()
        {
            states[currentState].Draw();
            states[currentState].DrawGUI();
        }

        #endregion

        #endregion

        public static void CreateStateType(SpriteBatch sb, ShapeDrawer pen, Dictionary<TextureIDs, Texture2D> textures)
        {
            // Dimensions.
            Padding screenPadding = new Padding(10);
            Padding centerPadding = new Padding(60);

            Vector2 bounds = new Vector2(135, 45);
            Vector2 screen = Game1.ScreenBounds;
            Vector2 center = Game1.ScreenCenter;

            // Buttons.
            Texture2D button = textures[TextureIDs.Button];
            Button start = new Button(Actions.Start, pen, Button.Positions.Center, new Vector2(0, centerPadding.GetY(-1)), bounds, button, "Start");
            Button options = new Button(Actions.Options, pen, Button.Positions.Center, null, bounds, button, "Options");
            Button scores = new Button(Actions.Scores, pen, Button.Positions.Center, new Vector2(0, centerPadding.GetY(1)), bounds, button, "Scores");
            Button exit = new Button(Actions.Quit, pen, Button.Positions.Center, new Vector2(0, centerPadding.GetY(2)), bounds, button, "Exit");

            Button dimensions = new Button(Actions.Dimensions, pen, Button.Positions.Center, null, null, button, "Change Dimensions");
            Button pause = new Button(Actions.Pause, pen, Button.Positions.BottomRight, screenPadding.Get(-1), bounds, button, "Pause");
            Button resume = new Button(Actions.Resume, pen, Button.Positions.Center, new Vector2(0, centerPadding.GetY(-2)), bounds, button, "Resume");
            Button quit = new Button(Actions.Quit, pen, Button.Positions.Center, new Vector2(0, centerPadding.GetY(-1)), null, button, "Quit to Windows");

            Button back = new Button(Actions.Back, pen, Button.Positions.BottomRight, screenPadding.Get(-1), bounds, button, "Back"); // Reused.
           
            // Asteroid texture set-up.
            List<Texture2D> asteroidTextures = new List<Texture2D>();
            asteroidTextures.Add(textures[TextureIDs.Asteroid1]);
            asteroidTextures.Add(textures[TextureIDs.Asteroid2]);
            asteroidTextures.Add(textures[TextureIDs.Asteroid3]);

            // Entities
            Asteroid asteroid = new Asteroid(pen, asteroidTextures[0], "Asteroid");
            TestMover test = new TestMover(pen, textures[TextureIDs.Test], Color.LimeGreen, _size: new Vector2(15, 15));
            TestMover test2 = new TestMover(pen, textures[TextureIDs.Test], Color.LimeGreen, _size: new Vector2(15, 15));
            test2.Tag = "!Test2";

            // Initialize the global particle generator for testing.
            // GlobalParticleGenerator.Initialize(textures[TextureIDs.Bullet], null, 10);

            // Entities under player control.
            test.PlayerControl = true;
            test2.PlayerControl = false;
            
            // Main State.
            StateType state = StateType.Main;
            AddState(state, new State(pen, Color.Black, Color.AntiqueWhite));
            AddButtonToState(state, start); // Start button.          
            AddButtonToState(state, exit); // Exit button.      
            AddButtonToState(state, options); // Options button.
            AddButtonToState(state, scores); // Score button.

            // Options State.
            state = StateType.Options;
            AddState(state, new State(pen, Color.Black, Color.Beige));
            AddButtonToState(state, back); // Back button.  
            AddButtonToState(state, dimensions); // Dimensions 

            // Arena State.
            state = StateType.Arena;
            AddState(state, new State(pen, Color.LimeGreen, Color.Black));
            AddButtonToState(state, pause);
            AddEntityToState(state, test);
            AddEntityToState(state, test2);
            AddEntityToState(state, asteroid);

            // Pause State.
            state = StateType.Pause;
            AddState(state, new State(pen, Color.Black, Color.Beige));
            AddButtonToState(state, resume);
            AddButtonToState(state, quit);
            
            // Gameover State.
            state = StateType.Gameover;
            AddState(state, new State(pen, Color.Black, Color.LavenderBlush));
            AddButtonToState(state, back); // Back button.  

            // Quit State.
            state = StateType.Quit;
            AddState(state, new State(pen, Color.Black, Color.BlueViolet));            
        }




	}
}
