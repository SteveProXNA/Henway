using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;
using Henway.Input;
using Henway.State.Abstract;

namespace Henway.Device
{
	public abstract class AbstractDeviceFactory
	{
		protected readonly HenwayGame game;

		protected AbstractDeviceFactory(HenwayGame game)
		{
			this.game = game;
		}

		public virtual void Initialize()
		{
			game.HenwayGameVariable.States[(Int32)game.HenwayGameVariable.CurrGameState].Initialize();
		}

		public virtual void LoadContent()
		{
			game.HenwayGameVariable.States[(Int32)game.HenwayGameVariable.CurrGameState].LoadContent();
		}

		public virtual void Update(GameTime gameTime)
		{
			game.HenwayGameVariable.States[(Int32)game.HenwayGameVariable.CurrGameState].Update(gameTime);
		}

		public virtual void Draw(GameTime gameTime)
		{
			game.SpriteBatch.Begin();
			game.HenwayGameVariable.States[(Int32)game.HenwayGameVariable.CurrGameState].Draw(gameTime);
			game.SpriteBatch.End();
		}

		public Song IntroSong()
		{
			Song song = LoadSong("SteveProIntro");
			return song;
		}

		public Song GameSong()
		{
			Song song = LoadSong("Revelation");

			// If game song not found in library then load from content.
			if (song == null)
			{
				const String musicFolder = "Music/";
				song = game.Content.Load<Song>(musicFolder + "Henway");
			}

			return song;
		}

		public virtual Song LoadSong(String name)
		{
			Song song = null;

			MediaLibrary library = new MediaLibrary();
			SongCollection songCollection = library.Songs;

			Int32 count = songCollection.Count;
			for (Int32 index = 0; index < count; index++)
			{
				String songName = songCollection[index].Name;
				Int32 compareTo = songName.CompareTo(name);
				if (compareTo != 0)
				{
					continue;
				}

				song = songCollection[index];
				break;
			}

			return song;
		}

		public virtual Boolean PlayIntro(Double timeSpan, Single endSong)
		{
			Boolean playIntro = timeSpan < endSong;
			return playIntro;
		}

		public virtual void LoadStorage()
		{
			// There is only one storage device for PC and Zune.
			// Therefore load storage device on background thread.
			try
			{
				IAsyncResult result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
				while (!result.IsCompleted)
				{
				}

				game.HenwayGameVariable.HenwayStorageDevice = Guide.EndShowStorageDeviceSelector(result);
				game.HenwayGameVariable.LoadHighScore();
			}
			catch
			{
			}
		}

		public virtual EnumGameState LoadStorageState()
		{
			return EnumGameState.GameState01Title;
		}

		public virtual void SaveStorage()
		{
			game.HenwayGameVariable.MaxLevel = game.HenwayGameVariable.CarLevel;
			game.HenwayGameVariable.SaveHighScore();
		}

		public virtual void SaveStorageEnd()
		{
		}

		public virtual EnumGameState SaveStorageState(EnumGameState saveEnumGameState)
		{
			return saveEnumGameState;
		}

		public virtual Boolean IsTrialMode()
		{
			return Guide.IsTrialMode;
		}

		public virtual void InitializeCarsCommon()
		{
			game.HenwayGameVariable.CarSprites[0].Velocity = new Vector2(0, 3);
			game.HenwayGameVariable.CarSprites[1].Velocity = new Vector2(0, 2);
			game.HenwayGameVariable.CarSprites[2].Velocity = new Vector2(0, -3);
			game.HenwayGameVariable.CarSprites[3].Velocity = new Vector2(0, -4);

			game.HenwayGameVariable.CarSprites[4].Velocity = new Vector2(0, 4);
			game.HenwayGameVariable.CarSprites[5].Velocity = new Vector2(0, 3);
			game.HenwayGameVariable.CarSprites[6].Velocity = new Vector2(0, -2);
			game.HenwayGameVariable.CarSprites[7].Velocity = new Vector2(0, -3);
		}

		public AbstractInputState InputState { get; set; }
		public Int32 PreferredBackBufferWidth { get; set; }
		public Int32 PreferredBackBufferHeight { get; set; }

		public Int32 GameWindowX { get; set; }
		public Int32 GameWindowY { get; set; }
		public Int32 GameWindowWidth{ get; set; }
		public Int32 GameWindowHeight { get; set; }

		public String ContentFolder { get; set; }
		public Single Scale { get; set; }

		public Single SongVolume { get; set; }
		public Single SoundEffectVolume { get; set; }

		public Single EndDelta { get; set; }
		public Single EndDelta2 { get; set; }
		public Single EndVolume { get; set; }

		public Int32 MaxCars { get; set; }
		public String[] CarTextureNames { get; set; }

		public Single[] CarLeftStart { get; set; }
		public Single ChickenCross { get; set; }

		public Int32[] RoadMapCheck { get; set; }
		public Int32[] RoadMapWidth { get; set; }

		public Int32[] RoadSignPosn { get; set; }
		public String[] RoadSignLocn { get; set; }
		public String[] RoadSignSize { get; set; }
		public String RoadSignHigh { get; set; }

		public String NextText { get; set; }
		public String BackText { get; set; }
		public String ContText { get; set; }
		public String RecordText { get; set; }
		public String[] HintText { get; set; }
	}

}