using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Henway.Collection;
using Henway.Command;
using Henway.Graphics;
using Henway.Sprite;
using Henway.Sprite.Action;
using Henway.State.Abstract;
using Henway.State.Concrete;

namespace Henway
{
	public class HenwayGameVariable
	{
		private readonly HenwayGame game;

		public AbstractGameState[] States { get; private set; }
		public EnumGameState CurrGameState { get; set; }
		public EnumGameState NextGameState { get; set; }

		public ChickenSprite ChickenSprite { get; set; }
		public IList<CarSprite> CarSprites { get; set; }
		public IList<RoadSignSprite> RoadSignMaster { get; set; }
		public IList<RoadSignSprite> RoadSignList { get; set; }
		public Dictionary<Int32, Int32> RoadSignDictionary { get; set; }

		public Texture2D HighwayTexture { get; set; }
		public Texture2D ChickenTexture { get; set; }
		public Texture2D ChickenOneTexture { get; set; }
		public Texture2D ChickenHeadTexture { get; set; }
		public Vector2 ChickenHeadPosition { get; set; }
		public IList<Texture2D> CarTexture { get; set; }
		public Texture2D BoostTexture { get; set; }

		public Texture2D[] RoadSignTexture { get; set; }
		public Texture2D WindowWidthTexture { get; set; }
		public Texture2D WindowHeightTexture { get; set; }

		public Texture2D DeathTexture { get; set; }
		public Texture2D BlankTexture { get; set; }
		public Rectangle BlankRectangle { get; set; }
		public Color BlankColor { get; set; }

		public SpriteFont ArialFont { get; set; }
		public SpriteFont ArialMediumFont { get; set; }
		public SpriteFont ArialSmallFont { get; set; }
		public SpriteFont VerdanaFont { get; set; }

		public Label HenwayLabel { get; set; }
		public Label[] TitleLabel { get; set; }
		public Label[] LevelLabel { get; set; }
		public Label[] RoadLabel { get; set; }
		public Label HighLabel { get; set; }
		public Label[] QuitLabel { get; set; }
		public Label[] OverLabel { get; set; }
		public Label[] GoalLabel { get; set; }

		public Label IntroTitleLabel { get; set; }
		public Label HighTitleLabel { get; set; }
		public Label LevelTitleLabel { get; set; }
		public Label StartTitleLabel { get; set; }
		public Label ExitTitleLabel { get; set; }
		public Label TrialTitleLabel { get; set; }
		public Label HintLevelLabel { get; set; }

		public Label[] StorageDeviceLabel { get; set; }
		public Label RecordBreakLabel { get; set; }
		public Label ContinueLabel { get; set; }
		public Label BoostLabel { get; set; }

		public Int32 CarLevel { get; set; }
		public Int32 MaxLevel { get; set; }
		public Int32 MaxLevelOld { get; set; }
		public Int32 MaxCars { get; set; }
		public StorageDevice HenwayStorageDevice { get; set; }

		public Int32 ActionSwerve { get; set; }
		public Int32 RoadComplete { get; set; }
		public Single RoadPercent { get; set; }
		public Int32[] RoadMap { get; set; }

		public Int32 NumberLives { get; set; }
		public Int32 DeathCar { get; set; }
		public Vector2 DeathVelocity { get; set; }
		public CarSprite DeathSprite { get; set; }

		public Random Random { get; set; }
		public Boolean LoadComplete { get; set; }
		public Double TimeConstant { get; private set; }

		public Boolean MasterQuit { get; set; }
		public Song IntroSong { get; set; }
		public Song GameSong { get; set; }

		public Int32[] MinValue { get; set; }
		public Int32[] MaxValue { get; set; }
		public Boolean GoalDisplay { get; set; }
		public Single GoalTimer { get; set; }

		public Rectangle BoostRectangle { get; set; }
		public Rectangle BoostRectangleBack { get; set; }
		public Vector4 BoostVector4 { get; set; }
		public Vector4 BoostVector4Back { get; set; }

		public Single BoostValue { get; set; }
		public Single BoostDelta { get; set; }

		public SoundEffect SquishEffect { get; set; }
		public SoundEffectInstance SquishEffectInstance { get; set; }

		public SoundEffect SquawkEffect { get; set; }
		public SoundEffectInstance SquawkEffectInstance { get; set; }

		public SoundEffect[] CarEngineEffect { get; set; }
		public SoundEffectInstance[] CarEngineEffectInstance { get; set; }

		public SoundEffect[] CarHornEffect { get; set; }
		public SoundEffectInstance[] CarHornEffectInstance { get; set; }

		public SoundEffect CarScreechEffect { get; set; }
		public SoundEffectInstance CarScreechEffectInstance { get; set; }

		public SoundEffect[] CarRevEffect { get; set; }
		public SoundEffectInstance[] CarRevEffectInstance { get; set; }

		public SoundEffect CelebrateEffect { get; set; }
		public SoundEffectInstance CelebrateEffectInstance { get; set; }

		public VariantList CommandsSave { get; set; }
		public VariantList CommandsSaveFrames { get; set; }

		//public List<ICommand> CommandsSave { get; set; }
		//public IList<Int32> CommandsSaveFrames { get; set; }

		public ICommand[][] Commands;
		public Int32[][] Frames;

		public Boolean UnlockError { get; set; }
		public SignedInGamer SignedInGamer { get; set; }
		public RenderTarget2D RenderTarget { get; set; }
		public Thread BackgroundThread { get; set; }

		public HenwayGameVariable(HenwayGame game)
		{
			this.game = game;

			// Set the game states.
			Type type = typeof(EnumGameState);
			FieldInfo[] info = type.GetFields(BindingFlags.Static | BindingFlags.Public);
			Int32 numberStates = info.Length;

			States = new AbstractGameState[numberStates];
			States[(Int32)EnumGameState.GameState00Intro] = new GameState00Intro(game);
			States[(Int32)EnumGameState.GameState01Drive] = new GameState01Drive(game);
			States[(Int32)EnumGameState.GameState01Title] = new GameState01Title(game);
			States[(Int32)EnumGameState.GameState02Level] = new GameState02Level(game);
			States[(Int32)EnumGameState.GameState03Play] = new GameState03Play(game);
			States[(Int32)EnumGameState.GameState04Cross] = new GameState04Cross(game);
			States[(Int32)EnumGameState.GameState05Finish] = new GameState05Finish(game);
			States[(Int32)EnumGameState.GameState06Dead] = new GameState06Dead(game);
			States[(Int32)EnumGameState.GameState07Over] = new GameState07Over(game);

			CurrGameState = EnumGameState.GameState00Intro;
			NextGameState = CurrGameState;

			MaxCars = game.DeviceFactory.MaxCars;
			CarTexture = new List<Texture2D>(MaxCars);
			CarSprites = new List<CarSprite>();

			RoadSignMaster = null;
			RoadSignList = null;
			RoadSignDictionary = null;

			Random = new Random();

			CommandsSave = new VariantList();
			CommandsSaveFrames = new VariantList();

			//CommandsSave = new List<ICommand>();
			//CommandsSaveFrames = new List<Int32>();

			TimeConstant = 5000;
			UnlockError = false;
		}

		public void BackgroundLoadContent()
		{
			if (BackgroundThread == null)
			{
				BackgroundThread = new Thread(BackgroundLoadContentAsync);
				BackgroundThread.Start();
			}
		}

		private void BackgroundLoadContentAsync()
		{
			// Get valid storage device to load and save high score.
			game.DeviceFactory.LoadStorage();

			// SignedIn Gamer.
			if (Gamer.SignedInGamers.Count > 0)
			{
				SignedInGamer = Gamer.SignedInGamers[0];
			}

			// Load background content.
			Int32 gameWindowX = game.DeviceFactory.GameWindowX;
			Int32 gameWindowY = game.DeviceFactory.GameWindowY;
			Int32 gameWindowWidth = game.DeviceFactory.GameWindowWidth;
			Int32 gameWindowHeight = game.DeviceFactory.GameWindowHeight;

			Rectangle bounds = new Rectangle(
				gameWindowX,
				gameWindowY,
				gameWindowWidth,
				gameWindowHeight
				);

			const String fontFolder = "Font/";
			const String imageFolder = "Image/";

			// Load chicken sprite.
			ChickenTexture = game.Content.Load<Texture2D>(imageFolder + "Chicken");
			ChickenOneTexture = game.Content.Load<Texture2D>(imageFolder + "ChickenOne");
			ChickenSprite = new ChickenSprite(
				ChickenTexture,
				ChickenOneTexture,
				game.DeviceFactory.Scale,
				Vector2.Zero,
				bounds,
				BoundsAction.Stop,
				8
				);

			// Load car sprites.
			String[] carTexture = game.DeviceFactory.CarTextureNames;
			for (Int32 index = 0; index < MaxCars; index++)
			{
				String assetName = imageFolder + carTexture[index];
				Texture2D texture = game.Content.Load<Texture2D>(assetName);
				CarTexture.Add(texture);
			}

			for (Int32 index = 0; index < MaxCars; index++)
			{
				CarSprite carSprite = new CarSprite(
					CarTexture[index],
					game.DeviceFactory.Scale,
					Vector2.Zero,
					Vector2.Zero,
					bounds,
					BoundsAction.Wrap,
					gameWindowY + gameWindowHeight,
					1
					);

				CarSprites.Add(carSprite);
			}

			// Load road sign sprites.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				RoadSignTexture = new Texture2D[2];
				RoadSignTexture[0] = game.Content.Load<Texture2D>(imageFolder + "RoadSignLarge");
				RoadSignTexture[1] = game.Content.Load<Texture2D>(imageFolder + "RoadSignSmall");

				Int32 length = game.DeviceFactory.RoadSignPosn.Length;
				RoadSignMaster = new List<RoadSignSprite>(length);
				RoadSignList = new List<RoadSignSprite>(length);
				RoadSignDictionary = new Dictionary<Int32, Int32>();

				for (Int32 index = 0; index < length; index++)
				{
					Int32 roadSignPosnX = game.DeviceFactory.RoadSignPosn[index];
					roadSignPosnX += game.DeviceFactory.GameWindowX;

					Vector2 position=  new Vector2(roadSignPosnX , 0);
					RoadSignSprite roadSignSprite = new RoadSignSprite(
						RoadSignTexture[0],
						game.DeviceFactory.Scale,
						position,
						bounds,
						1
						);

					RoadSignMaster.Add(roadSignSprite);
				}
			}

			// Load highway texture.
			HighwayTexture = game.Content.Load<Texture2D>(imageFolder + "Highway");

			// Load chicken head sprites.
			ChickenHeadTexture = game.Content.Load<Texture2D>(imageFolder + "ChickenHead");

			// Load black window bounds.
			WindowWidthTexture = game.Content.Load<Texture2D>(imageFolder + "WindowWidth");
			WindowHeightTexture = game.Content.Load<Texture2D>(imageFolder + "WindowHeight");

			DeathTexture = game.Content.Load<Texture2D>(imageFolder + "Death");
			BlankTexture = new Texture2D(game.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
			BlankTexture.SetData(new[] { Color.White });
			BlankColor = new Color(0, 0, 0, 128);

			// Load boost texture.
			BoostTexture = game.Content.Load<Texture2D>(imageFolder + "Boost");

			// Load sprite fonts.
			ArialFont = game.Content.Load<SpriteFont>(fontFolder + "Arial");
			ArialMediumFont = game.Content.Load<SpriteFont>(fontFolder + "ArialMedium");
			ArialSmallFont = game.Content.Load<SpriteFont>(fontFolder + "ArialSmall");
			VerdanaFont = game.Content.Load<SpriteFont>(fontFolder + "Verdana");

			// Load labels.
			HenwayLabel = new Label(game, VerdanaFont, Color.Yellow, Color.Black, LabelEffect.Shadow);
			TitleLabel = new Label[2];
			TitleLabel[0] = new Label(game, ArialFont, Color.White, Color.Black, LabelEffect.Shadow);
			TitleLabel[1] = new Label(game, ArialFont, Color.White, Color.Black, LabelEffect.Shadow);

			LevelLabel = new Label[2];
			LevelLabel[0] = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);
			LevelLabel[1] = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);

			RoadLabel = new Label[2];
			RoadLabel[0] = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);
			RoadLabel[1] = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);

			HighLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);

			QuitLabel = new Label[3];
			for (Int32 index = 0; index < 3; index++)
			{
				QuitLabel[index] = new Label(game, ArialSmallFont, Color.White, Color.White, LabelEffect.Outline);
			}

			OverLabel = new Label[2];
			OverLabel[0] = new Label(game, ArialFont, Color.White, Color.Black, LabelEffect.Shadow);
			OverLabel[1] = new Label(game, ArialFont, Color.White, Color.Black, LabelEffect.Shadow);

			GoalLabel = new Label[5];
			for (Int32 index = 0; index < 5; index++)
			{
				GoalLabel[index] = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);
			}

			IntroTitleLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);
			HighTitleLabel = new Label(game, ArialMediumFont, Color.YellowGreen, Color.Black, LabelEffect.Shadow);
			LevelTitleLabel = new Label(game, ArialMediumFont, Color.YellowGreen, Color.Black, LabelEffect.Shadow);
			StartTitleLabel = new Label(game, ArialMediumFont, Color.YellowGreen, Color.Black, LabelEffect.Shadow);
			ExitTitleLabel = new Label(game, ArialMediumFont, Color.YellowGreen, Color.Black, LabelEffect.Shadow);
			TrialTitleLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);
			HintLevelLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);

			StorageDeviceLabel = new Label[3];
			for (Int32 index = 0; index < 3; index++)
			{
				StorageDeviceLabel[index] = new Label(game, ArialSmallFont, Color.YellowGreen, Color.Black, LabelEffect.Outline);
			}

			RecordBreakLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.White, LabelEffect.Outline);
			ContinueLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);
			BoostLabel = new Label(game, ArialSmallFont, Color.Yellow, Color.Black, LabelEffect.Outline);

			// Load road map.
			RoadMap = new Int32[gameWindowWidth];
			Int32[] roadMapCheck = game.DeviceFactory.RoadMapCheck;
			Int32[] roadMapWidth = game.DeviceFactory.RoadMapWidth;

			Int32 count = 0;
			for (Int32 index = 0; index < gameWindowWidth; index++)
			{
				if (count < roadMapCheck.Length - 1)
				{
					if (index == roadMapWidth[count + 1])
					{
						count++;
					}
				}

				RoadMap[index] = roadMapCheck[count];
			}

			// Load min and max values for car velocities.
			MinValue = new[] { 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6 };
			MaxValue = new[] { 5, 5, 5, 5, 6, 6, 6, 6, 7, 7, 7, 7, 8, 8, 8, 8, 9, 9, 9, 9, 10 };

			// Load music.
			GameSong = game.DeviceFactory.GameSong();

			// Load frames.
			LoadCommands();

			// Complete load content.
			LoadComplete = true;

			GC.Collect();
		}

		public void LoadHighScore()
		{
			// Default max level.
			if (HenwayStorageDevice == null)
			{
				MaxLevel = 1;
				MaxLevelOld = MaxLevel;
				return;
			}

			StorageContainer storageContainer = null;
			try
			{
				storageContainer = HenwayStorageDevice.OpenContainer("Henway");
				String path = Path.Combine(storageContainer.Path, "Henway.txt");
				if (File.Exists(path))
				{
					// Read max level.
					StreamReader reader = null;
					try
					{
						reader = new StreamReader(path);
						String text = reader.ReadToEnd();

						try
						{
							MaxLevel = Int32.Parse(text);
							if (MaxLevel <= 0)
							{
								MaxLevel = 1;
							}
						}
						catch
						{
							MaxLevel = 1;
						}

						Boolean trialMode = game.DeviceFactory.IsTrialMode();
						if (trialMode && MaxLevel > 3)
						{
							MaxLevel = 3;
						}
					}
					catch
					{
						MaxLevel = 1;
					}
					finally
					{
						if (reader != null)
						{
							reader.Close();
							reader.Dispose();
						}
					}
				}
				else
				{
					MaxLevel = 1;
				}
			}
			catch
			{
				MaxLevel = 1;
			}
			finally
			{
				if (storageContainer != null)
				{
					storageContainer.Dispose();
				}
			}
		}

		public void SaveHighScore()
		{
			// If there was a problem with MaxLevel then don't save.
			if (MaxLevel < 1)
			{
				return;
			}

			if (HenwayStorageDevice == null)
			{
				return;
			}

			if (!HenwayStorageDevice.IsConnected)
			{
				return;
			}

			try
			{
				StorageContainer storageContainer = HenwayStorageDevice.OpenContainer("Henway");
				String path = Path.Combine(storageContainer.Path, "Henway.txt");

				// Write max level.
				using (StreamWriter writer = new StreamWriter(path))
				{
					String value = MaxLevel.ToString();
					writer.Write(value);

					writer.Flush();
					writer.Close();
				}

				storageContainer.Dispose();
			}
			catch
			{
			}
		}

		private void LoadCommands()
		{
			Commands = new ICommand[5][];
			Frames = new Int32[5][];

			LoadCommand(0);
			LoadCommand(1);
			LoadCommand(2);
		}

		private void LoadCommand(Int32 value)
		{
			String path = String.Format("{0}/Command/{1}.txt",
				game.DeviceFactory.ContentFolder,
				value
				);

			path = Path.Combine(StorageContainer.TitleLocation, path);

			IList<String> lines = new List<String>();
			using (StreamReader reader = new StreamReader(path))
			{
				String line = reader.ReadLine();
				while (line != null)
				{
					lines.Add(line);
					line = reader.ReadLine();
				}
			}

			Int32 maxCommand = lines.Count;
			Commands[value] = new ICommand[maxCommand];
			Frames[value] = new Int32[maxCommand];

			for (Int32 index = 0; index < maxCommand; index++)
			{
				String line = lines[index];
				String[] values = line.Split(new[] { ',' });

				Int32 frame = Convert.ToInt32(values[0]);
				Frames[value][index] = frame;

				Single velocityX = Convert.ToSingle(values[1]);
				Single velocityY = Convert.ToSingle(values[2]);
				Int32 curFrame = Convert.ToInt32(values[3]);
				Vector2 velocity = new Vector2(velocityX, velocityY);

				ICommand command = new MoveCommand(ChickenSprite, velocity, curFrame);
				Commands[value][index] = command;
			}
		}

		public void SaveCommands()
		{
			Int32 frame;

			Int32 count = CommandsSave.Count;
			for (Int32 index = 0; index < count; index++)
			{
				MoveCommand command = CommandsSave.GetAt<MoveCommand>(index);
				Single velocityX = ((MoveCommand)command).Velocity.X;
				Single velocityY = ((MoveCommand)command).Velocity.Y;
				Int32 curFrame = ((MoveCommand)command).CurFrame;

				frame = CommandsSaveFrames.GetAt<Int32>(index);
				String format = String.Format("{0},{1},{2},{3}",
					frame,
					velocityX,
					velocityY,
					curFrame
					);

				Console.WriteLine(format);
			}

			frame = 75;
		}

	}
}