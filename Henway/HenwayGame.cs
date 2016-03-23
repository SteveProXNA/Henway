using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Henway.Device;

namespace Henway
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class HenwayGame : Game
	{
		private readonly GraphicsDeviceManager graphics;
		public SpriteBatch SpriteBatch { get; private set; }

		public AbstractDeviceFactory DeviceFactory { get; set; }
		public HenwayGameVariable HenwayGameVariable { get; private set; }

		public HenwayGame()
		{
			graphics = new GraphicsDeviceManager(this);
			DeviceFactory = CreateDeviceFactory();

			graphics.PreferredBackBufferWidth = DeviceFactory.PreferredBackBufferWidth;
			graphics.PreferredBackBufferHeight = DeviceFactory.PreferredBackBufferHeight;
			Content.RootDirectory = DeviceFactory.ContentFolder;

			IGameComponent gamerServicesComponent = new GamerServicesComponent(this);
			Components.Add(gamerServicesComponent);

			// Set all the game variables.
			HenwayGameVariable = new HenwayGameVariable(this);
		}

		/// <henwayLogging>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			IsFixedTimeStep = false;
			IsMouseVisible = true;

			DeviceFactory.Initialize();
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			DeviceFactory.LoadContent();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			DeviceFactory.SaveStorageEnd();
			//HenwayGameVariable.SaveCommands();
			Content.Unload();
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Perform state transition check each frame:
			// If last frame we transitioned to new state then set at start of new current frame.
			if (HenwayGameVariable.NextGameState != HenwayGameVariable.CurrGameState)
			{
				HenwayGameVariable.CurrGameState = HenwayGameVariable.NextGameState;
				DeviceFactory.Initialize();
			}

			DeviceFactory.Update(gameTime);
			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);
			DeviceFactory.Draw(gameTime);

			base.Draw(gameTime);
		}

		internal void ShowSignIn()
		{
#if !ZUNE
			Guide.ShowSignIn(1, false);
#endif
		}

		internal void UnlockGame(PlayerIndex playerIndex)
		{
#if !ZUNE
			Guide.ShowMarketplace(playerIndex);
#endif
		}

		private AbstractDeviceFactory CreateDeviceFactory()
		{
			AbstractDeviceFactory deviceFactory;
#if WINDOWS
			deviceFactory = new WorkDeviceFactory(this);
#elif XBOX
			//deviceFactory = new WorkDeviceFactory(this);
			deviceFactory = new XboxDeviceFactory(this);
#elif ZUNE
			deviceFactory = new ZuneDeviceFactory(this);
#else
			throw new NotSupportedException("Device not found");
#endif
			return deviceFactory;
		}

		protected override void OnExiting(object sender, EventArgs args)
		{
			if (MediaPlayer.State == MediaState.Playing)
			{
				MediaPlayer.Stop();
			}

			// Cleanup.
			if (HenwayGameVariable.RenderTarget != null)
			{
				HenwayGameVariable.RenderTarget.Dispose();
			}
			if (HenwayGameVariable.BackgroundThread != null)
			{
				HenwayGameVariable.BackgroundThread.Abort();
			}
		}

	}
}