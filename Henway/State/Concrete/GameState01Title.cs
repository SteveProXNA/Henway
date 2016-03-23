using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Henway.Graphics;
using Henway.Sprite.Action;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState01Title : AbstractGameState
	{
		private Single offsetY;

		private Int32 frame;
		private Int32 maxCommand;
		private Int32 point;
		private Single timer;
		private Int32 value;
		private MenuLevelSelect menuLevelSelect;

		private ChickenState chickenState;
		private enum ChickenState
		{
			ChickenMove,
			ChickenWait
		};

		public GameState01Title(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			value = 0;

			// Play game song.
			if (MediaPlayer.State == MediaState.Stopped)
			{
				Song song = gameVariable.GameSong;
				if (song != null)
				{
					MediaPlayer.Volume = game.DeviceFactory.SongVolume;
					MediaPlayer.IsRepeating = true;
					MediaPlayer.Play(song);
				}
			}

			// Reset car velocities.
			gameVariable.CarLevel = 1;

			// Initialize welcome.
			Single newScale = scale * 4;
			Single welcomeOffsetX = gameWindowX + newScale;
			Single welcomeOffsetY = gameWindowY + newScale;
			gameVariable.IntroTitleLabel.SetPosition(welcomeOffsetX, welcomeOffsetY);

			// Set the Intro label.
			ResetGamerTag();

			// Initialize game label.
			InitializeHenwayLabel();

			// Initialize menu labels.
			InitializeMenuLabels();

			menuLevelSelect = MenuLevelSelect.Start;

			// Initialize Commands.
			InitializeCommands();
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			// Allow player to change landscape.
			inputState.ZuneControllerSwitch();

			// Check if player cancels error, this takes precedence.
			if (gameVariable.MasterQuit && gameVariable.UnlockError)
			{
				Boolean next = inputState.Next();
				if (next)
				{
					ResetQuestion();
				}
			}
			else
			{
				// Check if player starts.
				Boolean play = inputState.Play();
				if (play)
				{
					// User chooses explicitly to exit.
					if (menuLevelSelect == MenuLevelSelect.Exit)
					{
						game.Exit();
					}

					gameVariable.NextGameState = EnumGameState.GameState02Level;
					gameVariable.NumberLives = 3;

					// Initialize question to player.
					InitializeQuestion("Are you sure you want to quit?", game.DeviceFactory.NextText, game.DeviceFactory.BackText);
				}
			}

			// Update menu labels.
			if (!gameVariable.MasterQuit || !gameVariable.UnlockError)
			{
				UpdateMenuLabels();
			}

			// Update time interval for chicken wait.
			if (chickenState != ChickenState.ChickenWait)
			{
				return;
			}

			timer += gameTime.ElapsedGameTime.Milliseconds;
			if (timer > gameVariable.TimeConstant)
			{
				// Repeat command sequence.
				InitializeCommands();
			}
		}

		public override void UpdateSprites(GameTime gameTime)
		{
			// Update car sprites.
			UpdateCarsCommon(gameTime);

			// If chicken at wait state then quit.
			if (chickenState != ChickenState.ChickenMove)
			{
				return;
			}

			// Update chicken from commands.
			frame++;
			if (frame >= gameVariable.Frames[value][point])
			{
				frame = 0;
				gameVariable.Commands[value][point].Execute();

				point++;
				if (point >= maxCommand)
				{
					chickenState = ChickenState.ChickenWait;

					// Cause cars to finish.
					for (Int32 index = 0; index < gameVariable.MaxCars; index++)
					{
						gameVariable.CarSprites[index].BoundsAction = BoundsAction.Finish;
						gameVariable.CarSprites[index].CarAction = CarAction.None;
						gameVariable.CarSprites[index].CarSwerve = CarSwerve.None;
					}
				}
			}
		}

		public override void DrawChicken()
		{
			// Draw chicken sprite.
			DrawChickenCommon();
		}

		public override void DrawCars()
		{
			// Draw car sprites.
			DrawCarsCommon();
		}

		public override void DrawHud()
		{
			// Draw title.
			gameVariable.HenwayLabel.Draw();

			// Draw menu labels.
			DrawMenuLabels();

			// Draw quit question if applicable.
			base.DrawHud();
		}

		private void InitializeMenuLabels()
		{
			// Initialize Start label.
			String text = "START";
			Vector2 measureString = gameVariable.StartTitleLabel.GetPosition(text);

			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY = gameWindowY + (gameWindowHeight - measureString.Y) / 2;
			offsetY += scale * 48;
			gameVariable.StartTitleLabel.SetPosition(offsetX, offsetY);

			// Initialize Record Level label.
			text = "Record Level : " + gameVariable.MaxLevel;
			measureString = gameVariable.HighTitleLabel.GetPosition(text);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 48;
			gameVariable.HighTitleLabel.SetPosition(offsetX, offsetY);

			// Initialize Select Level label.
			gameVariable.CarLevel = gameVariable.MaxLevel;
			text = InitializeSelectLevel();
			measureString = gameVariable.LevelTitleLabel.GetPosition(text);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 48;
			gameVariable.LevelTitleLabel.SetPosition(offsetX, offsetY);

			// Initialize Exit label.
			text = "EXIT";
			measureString = gameVariable.ExitTitleLabel.GetPosition(text);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 48;
			gameVariable.ExitTitleLabel.SetPosition(offsetX, offsetY);

			// Initialize Trial label.
			Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
			if (isTrialMode)
			{
				text = "Sign in to unlock game";
				if (gameVariable.SignedInGamer != null)
				{
					text = "Press 'X' to unlock game";
				}

				measureString = gameVariable.TrialTitleLabel.GetPosition(text);

				offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
				offsetY = gameWindowY + (gameWindowHeight - measureString.Y);

				gameVariable.TrialTitleLabel.SetPosition(offsetX, offsetY);
			}
		}

		private void UpdateMenuLabels()
		{
			Boolean menuLeft = inputState.MenuLeft();
			if (menuLeft)
			{
				if (menuLevelSelect == MenuLevelSelect.Select)
				{
					if (gameVariable.CarLevel > 1)
					{
						gameVariable.CarLevel--;
						gameVariable.LevelTitleLabel.Text = InitializeSelectLevel();
					}
				}
			}

			Boolean menuRight = inputState.MenuRight();
			if (menuRight)
			{
				if (menuLevelSelect == MenuLevelSelect.Select)
				{
					if (gameVariable.CarLevel < gameVariable.MaxLevel)
					{
						gameVariable.CarLevel++;
						gameVariable.LevelTitleLabel.Text = InitializeSelectLevel();
					}
				}
			}

			if (menuLeft || menuRight)
			{
				return;
			}

			Boolean menuUp = inputState.MenuUp();
			if (menuUp)
			{
				menuLevelSelect--;
				if (menuLevelSelect < MenuLevelSelect.Start)
				{
					menuLevelSelect = MenuLevelSelect.Exit;
				}
			}

			Boolean menuDown = inputState.MenuDown();
			if (menuDown)
			{
				menuLevelSelect++;
				if (menuLevelSelect > MenuLevelSelect.Exit)
				{
					menuLevelSelect = MenuLevelSelect.Start;
				}
			}

			//if (menuUp || menuDown)
			//{
			//    return;
			//}
		}

		private void DrawMenuLabels()
		{
			// Draw Intro label.
			gameVariable.IntroTitleLabel.Draw();

			// Draw Start label.
			DrawHighlightLabel(gameVariable.StartTitleLabel, MenuLevelSelect.Start);

			// Draw Record Level label.
			gameVariable.HighTitleLabel.Draw();

			// Draw Select Level label.
			DrawHighlightLabel(gameVariable.LevelTitleLabel, MenuLevelSelect.Select);

			// Draw Exit label.
			DrawHighlightLabel(gameVariable.ExitTitleLabel, MenuLevelSelect.Exit);


			// Draw Trial label.
			Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
			if (isTrialMode)
			{
				if (!gameVariable.MasterQuit)
				{
					gameVariable.TrialTitleLabel.Draw();
				}
			}
		}

		private String InitializeSelectLevel()
		{
			return " Select Level : " + gameVariable.CarLevel;
		}

		private void DrawHighlightLabel(Label label, MenuLevelSelect testLevelSelect)
		{
			Color color = Color.YellowGreen;
			if (menuLevelSelect == testLevelSelect)
			{
				color = Color.White;
			}

			label.PrimaryColor = color;
			label.Draw();
		}

		private void InitializeCommands()
		{
			// Initialize chicken.
			InitializeChickenCommon();

			// Initialize cars.
			InitializeCarsCommon();
			game.DeviceFactory.InitializeCarsCommon();

			value++;
			if (value > 2)
			{
				value = 0;
			}

			maxCommand = gameVariable.Commands[value].GetLength(0);

			point = 0;
			frame = 0;
			timer = 0;

			chickenState = ChickenState.ChickenMove;
		}

		private enum MenuLevelSelect
		{
			Start,
			Select,
			Exit
		}

	}
}