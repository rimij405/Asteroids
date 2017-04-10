﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Asteroids.Entities;

namespace Asteroids.Tools
{
    // Enum
    public enum GenerationMode
    {
        Global,
        Local
    }

    
    public abstract class Generator : Entity
    {
        // Constants
        protected const int MAX_ENTITIES = 100;
        protected const int MIN_ENTITIES = 0;

        protected const int MIN_ITERATION = 1;

        protected const int MAX_SPEED = 1000;
        protected const int MAX_ANGSPEED = 7;
        protected const int MAX_ACCELERATION = 10;

        // Flags.
        private bool _initialized = false;

        // Fields.
        // Spawn area information.
        // Vector2 position; // Already pulled from "Entity".
        // float radius; // Already pulled from "Entity". The radius of the generator. Can be zero.
        // float scale; // Already pulled from "Entity".
        protected float generatorScale; // The scale applied to the particles generated, atop of the game scale. Ranges from 0.0f to 1.0f;
        
        // Spawn area and objects.
        protected Rectangle bounds; // Bounds to generate within.
        protected Vector2 direction; // Direction to launch the objects.
        protected List<Entity> objects; // The list of objects handled.
        protected bool useBounds; // Should bounds be used, or just the position?

        // Speed attributes.
        protected float baseSpeed; // The minimum speed to apply.
        protected float maxSpeed; // The maximum speed to apply.
        protected bool randomSpeed; // Should the speed be randomly generated?

        // Textures used for a particular object.
        protected List<Texture2D> textures; // Possible textures that a generated entity can have. Randomly generate the index and grab from the pool.

        // Generator attributes.
        protected int capacity; // The number of entities it can carry.
        protected int iteration; // Number of times to generate on every generation call.
		
        protected EntityType generatorType; // The type given to objects being generated by the generator.

        // Spawn information.
        protected float speedVal = 1000f;
        protected float angSpeedVal = 7;
        protected float accelVal = 10;


		// Constructor.
		public Generator(State _state, List<Texture2D> _images, EntityType _generatorType)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", null, 1, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
			Initialize(_generatorType, null, null, 100, 1, null, null, _images.ToArray<Texture2D>());
		}


        public Generator(State _state, Texture2D _image, EntityType _generatorType)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", null, 1, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
			Initialize(_generatorType, null, null, 100, 1, null, null, _image);
		}

		public Generator(State _state, List<Texture2D> _images, EntityType _generatorType, Rectangle? _bounds = null, Vector2? _dir = null,
			int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", null, 1, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
			Initialize(_generatorType, _bounds, _dir, _max, _iteration, null, null, _images.ToArray<Texture2D>());
		}

		public Generator(State _state, Texture2D _image, EntityType _generatorType, Rectangle? _bounds = null, Vector2? _dir = null,
            int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", null, 1, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
			Initialize(_generatorType, _bounds, _dir, _max, _iteration, null, null, _image);
        }

		public Generator(State _state, List<Texture2D> _images, EntityType _generatorType, Vector2? _pos = null, Vector2? _dir = null, Vector2? _size = null,
			int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", _pos, _size, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
            Initialize(_generatorType, _pos, _dir, _size, _max, _iteration, null, null, _images.ToArray<Texture2D>());
        }
		public Generator(State _state, Texture2D _image, EntityType _generatorType, Vector2? _pos = null, Vector2? _dir = null, Vector2? _size = null,
			int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION)
			: base(_state, GlobalManager.Pen.Dot, "[" + _generatorType + "] Generator", _pos, _size, 0, ScrollBehavior.Null, CollisionBehavior.Null, false, false, false, null, null)
		{
			Initialize(_generatorType, _pos, _dir, _size, _max, _iteration, null, null, _image);
		}

		public Vector2 GetPosition()
        {
            Random r = InputManager.RNG;

            float x = r.Next(bounds.X, bounds.X + bounds.Width + 1);
            float y = r.Next(bounds.Y, bounds.Y + bounds.Height + 1);

            return new Vector2(x, y);
        }

        public Vector2 GetDirection()
        {
            if (IsEmpty(this.direction))
            {
                // Generate new values.
                Random r = InputManager.RNG;
                float x = r.Next(1, (int)GlobalManager.ScreenBounds.X + 1);
                float y = r.Next(1, (int)GlobalManager.ScreenBounds.Y + 1);

                return Vector2.Normalize(new Vector2(x, y));
            }
            else
            {
                return this.direction;
            }
        }

        /// <summary>
        /// Spawn creates an entity at a given location and sets up any initial attributes.
        /// </summary>
        public abstract void SpawnEntity(int lifetime = -1);

        // A generator should take care of its entities update calls.
        // public abstract override void Update(float delta;

        // A generator should take care of its entities update calls.
        // public abstract override void UpdateGUI(float delta);

        // A generator should take care of its entities draw calls.
        // public abstract override void Draw();

        // A generator should take care of its entities draw calls. It can also add its own debug messages to the top if need be.
        // public abstract override void DrawGUI();

        // Stop the generator and its artifacts.
        // public abstract void Stop();
        
        // Start the generator.
        // public abstract void Start();

        /* public void SpawnParticle(int life = 10)
        {
            Vector2 pos = GetPosition();
            Vector2 vel = GetDirection();

            // Create the entity.
            Particle a = new Particle(this.pen, this.image, "Particle [Generator]");
            
            // Set its attributes.
            a.MaxSpeed = speedVal;
            a.MaxAcceleration = accelVal;
            a.MaxAngularSpeed = angSpeedVal;
            a.CollisionsOn = collider;
            a.Scrollable = scrollable;

            // Spawn and push the entity.
            a.Spawn(pos, bounds, life);
            a.Push(vel);
            a.Spin(true);
            
            // Add the entity to the generator list.
            Add(a);
        }

        public void SpawnAsteroid()
        {
            Random r = InputManager.RandomNumberGenerator;

            Vector2 pos = GetPosition();
            Vector2 vel = GetDirection();
            
            // Create attributes.
            int level = r.Next(Asteroid.MIN_LEVEL, Asteroid.MAX_LEVEL);
            int health = Asteroid.BASE_HEALTH;
            int value = Asteroid.BASE_VALUE;

            // Create the entity.

            Asteroid a = new Asteroid(this.pen, this.image, "Asteroid [Generator]", null, null, 0, level, health, value, scrollable);

            // Set its attributes.
            a.MaxSpeed = speedVal;
            a.MaxAcceleration = accelVal;
            a.MaxAngularSpeed = angSpeedVal;
            a.CollisionsOn = collider;

            // Spawn, push, and spin the entity.
            a.Spawn(pos);
            a.Push(vel);
            a.Spin(vel);

            // Add the entity to the generator list.
            Add(a);
        } */

        public void Spawn(int lifetime)
        {   
            // Stop if at max capacity.
            if (MaxCapacity())
            {
                return;
            }

            // Loop the indicated amount of times.
            for (int i = 0; i < iteration; i++)
            {
                // Stop if at max capacity.
                if (MaxCapacity())
                {
                    break;
                }
                else
                {
                    SpawnEntity(lifetime);
                }
            }
        }
        
        public void Add(Entity e = null)
        {
            if (e != null)
            {
                objects.Add(e);
            }
        }

        public bool Empty()
        {
            return (objects == null) || (objects.Count() == 0);
        }

        public bool MaxCapacity()
        {
            return ((!Empty()) && (objects.Count() == capacity));
        }

        private bool HasDirection()
        {
            if (IsEmpty(direction))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        protected virtual void Initialize(EntityType _generatorType, Rectangle? _bounds = null, Vector2? _dir = null, int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION, List<ScrollBehavior> _scrollModes = null, List<CollisionBehavior> _collideModes = null, params Texture2D[] _textures)
		{
			this.Type = EntityType.Generator;

			this.visible = false;
            objects = new List<Entity>();

			foreach (ScrollBehavior behavior in _scrollModes)
			{
				AddScrollBehavior(behavior);
			}

			foreach (CollisionBehavior behavior in _collideModes)
			{
				AddCollisionBehavior(behavior);
			}

			textures = new List<Texture2D>();

			foreach (Texture2D tex in _textures)
			{
				this.textures.Add(tex);
			}

			SetMaximum(_max);
            SetIteration(_iteration);
            SetPosition(_bounds);
            SetDirection(_dir);
            SetSpawnArea(_bounds);
        }

        protected virtual void Initialize(EntityType _generatorType, Vector2? _pos = null, Vector2? _dir = null, Vector2? _dim = null, int _max = MAX_ENTITIES, int _iteration = MIN_ITERATION, List<ScrollBehavior> _scrollModes = null, List<CollisionBehavior> _collideModes = null, params Texture2D[] _textures)
        {
            this.visible = false;

			foreach (ScrollBehavior behavior in _scrollModes)
			{
				AddScrollBehavior(behavior);
			}

			foreach (CollisionBehavior behavior in _collideModes)
			{
				AddCollisionBehavior(behavior);
			}
			
			textures = new List<Texture2D>();

			foreach (Texture2D tex in _textures)
			{
				this.textures.Add(tex);
			}

			SetMaximum(_max);
            SetIteration(_iteration);
            SetPosition(_pos);
            SetDirection(_dir);
            SetSpawnArea(_pos, _dim);
        }

        // Set the type.
       /* protected virtual void SetType(EntityType _type)
        {
            switch (_type)
            {
                case EntityType.Asteroid:
                    this.speedVal = 800;
                    this.accelVal = 15;
                    this.angSpeedVal = 5;
                    this.collider = true;
                    this.moveable = true;
                    break;
                case EntityType.Particle:
                    this.speedVal = 1500;
                    this.accelVal = 15;
                    this.angSpeedVal = 5;
                    this.collider = false;
                    this.moveable = true;
                    break;
                case EntityType.Star:
                    this.speedVal = 100;
                    this.accelVal = 2;
                    this.angSpeedVal = 1;
                    this.collider = false;
                    this.moveable = true;
                    break;
                case EntityType.Test:
                    this.collider = false;
                    this.moveable = true;
                    break;
                default:
                    return;
            }
        } */

        // Set the capacity of the generator.
        protected virtual void SetMaximum(int _max)
        {
            this.capacity = MathHelper.Clamp(_max, MIN_ENTITIES, MAX_ENTITIES);
        }

        // Generate this many artifacts at one time.
        protected virtual void SetIteration(int _spawn)
        {
            this.iteration = MathHelper.Clamp(_spawn, MIN_ITERATION, capacity);
        }
        
        // Position items are spawned from. If the generator supplies a radius, it will be centered around this point.
        protected virtual void SetPosition(Rectangle? _bounds = null)
        {
            if (_bounds == null)
            {
                SetPosition(_pos: null);
            }
            else
            {
                SetPosition(new Vector2(((Rectangle)_bounds).X, ((Rectangle)_bounds).Y));
            }
        }

        // Position items are spawned from. If the generator supplies a radius, it will be centered around this point.
        protected virtual void SetPosition(Vector2? _pos = null)
        {
            if (IsEmpty(_pos))
            {
                this.position = GlobalManager.ScreenCenter;
            }
            else
            {
                this.position = (Vector2)_pos;
            }
        }

        // Direction items spawned will head in. Can be null.
        protected virtual void SetDirection(Vector2? _dir = null)
        {
            this.direction = Vector2.Normalize(GlobalManager.ScreenBounds - GlobalManager.ScreenBounds);

            if (IsEmpty(_dir))
            {
                this.direction = new Vector2(0, 0);
            }
            else
            {
                this.direction = Vector2.Normalize((Vector2)_dir);
            }
        }
        
        // The legal area in which objects are allowed to spawn from.
        protected virtual void SetSpawnArea(Vector2? _pos = null, Vector2? _dim = null)
        {
            int x = 0;
            int y = 0;
            int width = 1;
            int height = 1;

            Point pos = new Point(x, y);
            Point dim = new Point(width, height);

            Random r = InputManager.RNG;
            Vector2 screen = GlobalManager.ScreenBounds;

            if (IsEmpty(_pos))
            {
                if (IsEmpty(this.position))
                {
                    x = r.Next(0, (int)screen.X);
                    y = r.Next(0, (int)screen.Y);
                }
                else
                {
                    x = (int)this.position.X;
                    y = (int)this.position.Y;
                }
            }
            else
            {
                x = (int)((Vector2)_pos).X;
                y = (int)((Vector2)_pos).Y;
            }

            // Clamp the generated values.
            x = (int)MathHelper.Clamp(x, 0, screen.X);
            y = (int)MathHelper.Clamp(y, 0, screen.Y);
            pos = new Point(x, y);

            if (IsEmpty(_dim))
            {
                // Set to the space right and down of the pos Point.
                width = (int)(screen.X - pos.X);
                height = (int)(screen.Y - pos.Y);
            }
            else
            {
                width = (int)((Vector2)_dim).X;
                height = (int)((Vector2)_dim).Y;
            }

            // Clamp the dimensions.
            width = (int)MathHelper.Clamp(width, 1, screen.X);
            height = (int)MathHelper.Clamp(height, 1, screen.Y);
            dim = new Point(width, height);            

            SetSpawnArea(new Rectangle(pos, dim));
        }

        // The legal area in which objects are allowed to spawn from.
        protected virtual void SetSpawnArea(Rectangle? _bounds = null)
        {
            if (_bounds == null)
            {
                SetSpawnArea(null, null);
            }
            else
            {
                this.bounds = (Rectangle)_bounds;
            }
        }
    }
}
