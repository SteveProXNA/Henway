using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Input;

namespace Henway.Device
{
	public class ZuneDeviceFactory : AbstractDeviceFactory
	{
		public ZuneDeviceFactory(HenwayGame game)
			: base(game)
		{
			InputState = new ZuneInputState();

			PreferredBackBufferWidth = 640;
			PreferredBackBufferHeight = 480;

			GameWindowX = 0;
			GameWindowY = 0;
			GameWindowWidth = 320;
			GameWindowHeight = 240;

			ContentFolder = "ContentLO";
			Scale = 0.5f;

			SongVolume = 0.9f;
			SoundEffectVolume = 0.7f;

			EndDelta = 0.0025f;
			EndDelta2 = 0.005f;
			EndVolume = 0.5f;

			MaxCars = 4;
			CarTextureNames = new[] { "Car1", "Car2", "Car3", "Car4" };
			CarLeftStart = new[] { 57.5f, 115, 172.5f, 230 };
			ChickenCross = 280.0f;

			RoadMapCheck = new[] { 11, 1, 13, 2, 15, 3, 17, 4, 19 };
			RoadMapWidth = new[] { 0, 37, 86, 95, 135, 152, 201, 210, 250 };

			RoadSignPosn = null;
			RoadSignLocn = null;
			RoadSignSize = null;
			RoadSignHigh = null;

			NextText = " 'Play' = Yes";
			BackText = " 'Back' = No";
			ContText = "Press 'Play' to continue";
			RecordText = "Broken record!";
			HintText = new[] { "HINT: Press 'Play' for Boost Power!", "HINT: Rest btwn lanes to avoid cars!", "HINT: Cars will drive faster!" };
		}

		public override void LoadContent()
		{
			game.HenwayGameVariable.RenderTarget = new RenderTarget2D(
				game.GraphicsDevice,
				GameWindowWidth,
				GameWindowHeight,
				0,
				SurfaceFormat.Color);

			base.LoadContent();
		}

		public override void Draw(GameTime gameTime)
		{
			// Determine the angle to render.
			Single angle = MathHelper.ToRadians(90.0f);
			ZuneController zuneController = InputState.ZuneController;
			if (zuneController == ZuneController.LeftLandscape)
			{
				angle = MathHelper.ToRadians(270.0f);
			}

			// Set render target.
			game.GraphicsDevice.SetRenderTarget(0, game.HenwayGameVariable.RenderTarget);
			game.GraphicsDevice.Clear(Color.Black);

			// Draw the scene.
			base.Draw(gameTime);

			// Render the updated scene.
			game.GraphicsDevice.SetRenderTarget(0, null);
			game.SpriteBatch.Begin();

			game.SpriteBatch.Draw(
				game.HenwayGameVariable.RenderTarget.GetTexture(),
				new Vector2(GameWindowHeight * Scale, GameWindowWidth * Scale),
				null,
				Color.White,
				angle,
				new Vector2(GameWindowWidth * Scale, GameWindowHeight * Scale),
				1f,
				SpriteEffects.None,
				0);

			game.SpriteBatch.End();
		}

		public override bool IsTrialMode()
		{
			// Zune is currently always full mode.
			return false;
		}

		public override void InitializeCarsCommon()
		{
			game.HenwayGameVariable.CarSprites[0].Velocity = new Vector2(0, 3);
			game.HenwayGameVariable.CarSprites[1].Velocity = new Vector2(0, 2);
			game.HenwayGameVariable.CarSprites[2].Velocity = new Vector2(0, -3);
			game.HenwayGameVariable.CarSprites[3].Velocity = new Vector2(0, -4);
		}

		public override void SaveStorage()
		{
			game.HenwayGameVariable.MaxLevel = game.HenwayGameVariable.CarLevel;
		}

		public override void SaveStorageEnd()
		{
			if (game.HenwayGameVariable.MaxLevel > game.HenwayGameVariable.MaxLevelOld)
			{
				game.HenwayGameVariable.SaveHighScore();
			}
		}
	}
}