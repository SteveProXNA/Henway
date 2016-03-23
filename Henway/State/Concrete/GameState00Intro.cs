using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Henway.Graphics;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState00Intro : AbstractGameState
	{
		private Boolean playIntro = true;
		private SpriteFont chillerFont;
		private SpriteFont chillerSmallFont;

		private Label mainLabel;
		private Label startLabel;
		private Label yearLabel;
		private Single alpha;
		private Single alpha2;
		private Double timeSpan;
		private Single delta = 0.001f;
		private Single timer;
		private Boolean showStart;
		private Boolean hideStart;
		private const Single endSong = 28000;

		public GameState00Intro(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			delta = 0.001f / game.DeviceFactory.Scale;
			timer = 0;
			showStart = true;
			hideStart = false;
		}

		public override void LoadContent()
		{
			// Load all intro content immediately.
			const String fontFolder = "Font/";

			chillerFont = game.Content.Load<SpriteFont>(fontFolder + "Chiller");
			chillerSmallFont = game.Content.Load<SpriteFont>(fontFolder + "ChillerSmall");

			// Play intro song.
			gameVariable.IntroSong = game.DeviceFactory.IntroSong();
			Song song = gameVariable.IntroSong;
			if (song != null)
			{
				MediaPlayer.Volume = 1.0f;
				MediaPlayer.Play(song);
			}

			// Set label variables.
			mainLabel = new Label(game, chillerFont, Color.Red, Color.White, LabelEffect.Shadow);
			startLabel = new Label(game, chillerSmallFont, Color.White, Color.Black, LabelEffect.None);
			yearLabel = new Label(game, chillerSmallFont, Color.White, Color.Black, LabelEffect.None);

			// Set main label
			Vector2 measureString = mainLabel.GetPosition("stevepro solutions");
			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = gameWindowY + (gameWindowHeight - measureString.Y) / 2;
			mainLabel.SetPosition(offsetX, offsetY);

			// Set start label.
			measureString = startLabel.GetPosition(" PRESS  START ");
			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY = gameWindowY + (gameWindowHeight - measureString.Y);
			startLabel.SetPosition(offsetX, offsetY);

			// Set year label.
			measureString = yearLabel.GetPosition("© 2007 ");
			offsetX = gameWindowX + gameWindowWidth - measureString.X;
			offsetY = gameWindowY + (gameWindowHeight - measureString.Y);
			yearLabel.SetPosition(offsetX, offsetY);

			// Load audio.
			LoadAudio();

			// Load all game content asynchronously.
			gameVariable.BackgroundLoadContent();
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			// Cannot exit while content loading.
			if (!gameVariable.LoadComplete)
			{
				return;
			}

			// Check if player exits.
			Boolean exit = inputState.Exit();
			if (exit)
			{
				game.Exit();
			}

			// Check if player starts.
			Boolean start = inputState.Start();
			if (start)
			{
				playIntro = false;
				hideStart = true;
			}
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (gameVariable.LoadComplete && playIntro)
			{
				playIntro = game.DeviceFactory.PlayIntro(timeSpan, endSong);
			}

			if (playIntro)
			{
				alpha += delta;
				alpha2 += delta;
				if (alpha >= 1.0f)
				{
					alpha = 1.0f;
					alpha2 = 1.0f;
				}
			}
			else
			{
				alpha -= deviceFactory.EndDelta;
				alpha2 -= deviceFactory.EndDelta2;
				MediaPlayer.Volume = alpha;
				if (alpha <= deviceFactory.EndVolume)
				{
					alpha = 0.0f;
					alpha2 = 0.0f;

					MediaPlayer.Volume = alpha;
					MediaPlayer.Stop();

					// If intro complete and no player index chosen then default to One.
					if (game.DeviceFactory.InputState.PlayerIndex == null)
					{
						// Windows and Zune only have PlayerIndex One.
						game.DeviceFactory.InputState.PlayerIndex = PlayerIndex.One;
					}

					// Assume next state is the Title.
					EnumGameState nextEnumGameState = game.DeviceFactory.LoadStorageState();
					gameVariable.NextGameState = nextEnumGameState;
				}
			}

			if (gameVariable.LoadComplete)
			{
				timer += gameTime.ElapsedGameTime.Milliseconds;
				if (timer > 800)
				{
					timer = 0;
					showStart = !showStart;
				}
			}
		}

		public override void DrawHighway()
		{
		}

		public override void DrawHud()
		{
			Single smoothValue = MathHelper.SmoothStep(0, 1, alpha2);

			Vector4 vectorWhite = new Vector4(255, 255, 255, smoothValue);
			Color white = new Color(vectorWhite);

			Vector4 vectorRed = new Vector4(255, 0, 0, smoothValue);
			Color red = new Color(vectorRed);

			mainLabel.PrimaryColor = red;
			mainLabel.SecondaryColor = white;
			startLabel.PrimaryColor = white;
			yearLabel.PrimaryColor = white;

			// Draw labels.
			mainLabel.Draw();

			if (gameVariable.LoadComplete)
			{
				if (showStart && !hideStart)
				{
					startLabel.Draw();
				}

				yearLabel.Draw();
			}
		}

		private void LoadAudio()
		{
			// Load audio.
			const String audioFolder = "Audio/";

			// Squish.
			game.HenwayGameVariable.SquishEffect = game.Content.Load<SoundEffect>(audioFolder + "Squish");
			game.HenwayGameVariable.SquishEffectInstance = game.HenwayGameVariable.SquishEffect.CreateInstance();
			game.HenwayGameVariable.SquishEffectInstance.Volume = 1.0f;

			// Squawk.
			game.HenwayGameVariable.SquawkEffect = game.Content.Load<SoundEffect>(audioFolder + "Squawk");
			game.HenwayGameVariable.SquawkEffectInstance = game.HenwayGameVariable.SquawkEffect.CreateInstance();
			game.HenwayGameVariable.SquawkEffectInstance.Volume = 1.0f;

			// Car Engine.
			game.HenwayGameVariable.CarEngineEffect = new SoundEffect[3];
			game.HenwayGameVariable.CarEngineEffect[0] = game.Content.Load<SoundEffect>(audioFolder + "CarEngine1");
			game.HenwayGameVariable.CarEngineEffect[1] = game.Content.Load<SoundEffect>(audioFolder + "CarEngine2");
			game.HenwayGameVariable.CarEngineEffect[2] = game.Content.Load<SoundEffect>(audioFolder + "CarEngine3");

			game.HenwayGameVariable.CarEngineEffectInstance = new SoundEffectInstance[3];
			for (Int32 index = 0; index < 3; index++)
			{
				game.HenwayGameVariable.CarEngineEffectInstance[index] = game.HenwayGameVariable.CarEngineEffect[index].CreateInstance();
			}

			// Car Horn.
			game.HenwayGameVariable.CarHornEffect = new SoundEffect[2];
			game.HenwayGameVariable.CarHornEffect[0] = game.Content.Load<SoundEffect>(audioFolder + "CarHorn1");
			game.HenwayGameVariable.CarHornEffect[1] = game.Content.Load<SoundEffect>(audioFolder + "CarHorn2");

			game.HenwayGameVariable.CarHornEffectInstance = new SoundEffectInstance[2];
			for (Int32 index = 0; index < 2; index++)
			{
				game.HenwayGameVariable.CarHornEffectInstance[index] = game.HenwayGameVariable.CarHornEffect[index].CreateInstance();
			}

			// Car Screech.
			game.HenwayGameVariable.CarScreechEffect = game.Content.Load<SoundEffect>(audioFolder + "CarScreech");
			game.HenwayGameVariable.CarScreechEffectInstance = game.HenwayGameVariable.CarScreechEffect.CreateInstance();

			// Car Rev.
			game.HenwayGameVariable.CarRevEffect = new SoundEffect[2];
			game.HenwayGameVariable.CarRevEffect[0] = game.Content.Load<SoundEffect>(audioFolder + "CarRev1");
			game.HenwayGameVariable.CarRevEffect[1] = game.Content.Load<SoundEffect>(audioFolder + "CarRev2");

			game.HenwayGameVariable.CarRevEffectInstance = new SoundEffectInstance[2];
			for (Int32 index = 0; index < 2; index++)
			{
				game.HenwayGameVariable.CarRevEffectInstance[index] = game.HenwayGameVariable.CarRevEffect[index].CreateInstance();
			}

			// Celebrate.
			game.HenwayGameVariable.CelebrateEffect = game.Content.Load<SoundEffect>(audioFolder + "Celebrate");
			game.HenwayGameVariable.CelebrateEffectInstance = game.HenwayGameVariable.CelebrateEffect.CreateInstance();
		}

	}
}