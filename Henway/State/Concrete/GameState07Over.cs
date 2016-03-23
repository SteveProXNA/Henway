using System;
using Microsoft.Xna.Framework;
using Henway.Sprite.Action;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState07Over : AbstractGameState
	{
		private Double timeSpan;

		public GameState07Over(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize cars.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				gameVariable.CarSprites[index].BoundsAction = BoundsAction.Finish;
				gameVariable.CarSprites[index].CarAction = CarAction.None;
			}

			// Initialize Game Over label.
			String text = "GAME ";
			Vector2 measureString = gameVariable.OverLabel[0].GetPosition(text);

			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = (Single)(gameWindowY + (gameWindowHeight - measureString.Y) / 2.25);
			gameVariable.OverLabel[0].SetPosition(offsetX, offsetY);

			text = "OVER ";
			measureString = gameVariable.OverLabel[1].GetPosition(text);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 64;
			gameVariable.OverLabel[1].SetPosition(offsetX, offsetY);

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

			// Initialize Continue label.
			text = game.DeviceFactory.ContText;
			measureString = gameVariable.ContinueLabel.GetPosition(text);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY = gameWindowY + (gameWindowHeight - measureString.Y);

			gameVariable.ContinueLabel.SetPosition(offsetX, offsetY);

			// Reset time.
			timeSpan = 0;
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			// Check if player cancels quit.
			CheckCancelQuit();

			Boolean next = inputState.Next();
			if (!next)
			{
				return;
			}

			// Check if player accepts quit.
			if (gameVariable.MasterQuit)
			{
				if (!gameVariable.UnlockError)
				{
					gameVariable.NextGameState = EnumGameState.GameState01Title;
				}
				else
				{
					ResetQuestion();
				}
			}
			else
			{
				Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
				if (isTrialMode)
				{
					gameVariable.NextGameState = EnumGameState.GameState01Title;
				}
				else
				{
					// Check if player press 'A' to continue.
					InitializeCarsCommon();
					gameVariable.NumberLives = 3;
					gameVariable.NextGameState = EnumGameState.GameState03Play;
				}
			}
		}

		public override void UpdateSprites(GameTime gameTime)
		{
			// Update car sprites.
			UpdateCarsCommon(gameTime);
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSpan > gameVariable.TimeConstant * 2)
			{
				gameVariable.NextGameState = EnumGameState.GameState01Title;
			}
		}

		public override void DrawCars()
		{
			// Draw car sprites.
			DrawCarsCommon();
		}

		public override void DrawHud()
		{
			if (!gameVariable.MasterQuit)
			{
				gameVariable.OverLabel[0].Draw();
				gameVariable.OverLabel[1].Draw();
			}

			// Draw Trial/Continue label.
			if (!gameVariable.MasterQuit)
			{
				Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
				if (isTrialMode)
				{
					gameVariable.TrialTitleLabel.Draw();
				}
				else
				{
					gameVariable.ContinueLabel.Draw();
				}
			}

			// Draw quit question if applicable.
			base.DrawHud();
		}

	}
}