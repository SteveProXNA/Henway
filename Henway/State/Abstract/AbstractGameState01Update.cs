using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Henway.Sprite;
using Henway.Sprite.Action;

namespace Henway.State.Abstract
{
	/// <summary>
	/// Template method implementations for Update.
	/// </summary>
	public abstract partial class AbstractGameState
	{
		public virtual void UpdateInput(GameTime gameTime)
		{
			inputState.Update();

			// Cannot process any input during intro.
			if (gameVariable.CurrGameState == EnumGameState.GameState00Intro)
			{
				return;
			}

			if (gameVariable.CurrGameState != EnumGameState.GameState01Title)
			{
				if (!gameVariable.MasterQuit)
				{
					Boolean quit = inputState.Quit();
					if (quit)
					{
						inputState.ResetMotor();
						StopSoundEffectVolume();
						ResetQuestion();
						return;
					}
				}
			}

			// Cannot unlock game if already unlocked!
			Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
			if (!isTrialMode)
			{
				return;
			}

			// Attempt to unlock game.
			UnlockGame();
		}

		public virtual void UpdateSprites(GameTime gameTime)
		{
		}

		public virtual void UpdateGeneral(GameTime gameTime)
		{
		}

		protected void UpdateCarsCommon(GameTime gameTime)
		{
			// Update car sprites.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				CarSprite carSprite = gameVariable.CarSprites[index];

				// Check for car acceleration or deceleration.
				if (carSprite.CarAction != CarAction.None)
				{
					UpdateCarsVelocity(carSprite, gameTime);
				}

				// Check for car swerve left or right.
				if (carSprite.CarSwerve != CarSwerve.None)
				{
					UpdateCarsPosition(carSprite, gameTime);
				}

				carSprite.Update();
			}
		}

		protected void CheckCancelQuit()
		{
			if (gameVariable.MasterQuit && !gameVariable.UnlockError)
			{
				Boolean back = inputState.Back();
				if (back)
				{
					ResetQuestion();
				}
			}
		}

		protected static void UpdateCarsVelocity(CarSprite carSprite, GameTime gameTime)
		{
			if (carSprite.CarAction != CarAction.Timer)
			{
				Vector2 velocity = carSprite.Velocity;
				velocity *= carSprite.Delta;

				carSprite.Velocity = velocity;

				// Transition to next car velocity state.
				Single carVelocityY = Math.Abs(carSprite.Velocity.Y);
				Single carEndVelocityY = Math.Abs(carSprite.EndVelocity.Y);

				if (carSprite.Delta > 1 && carVelocityY >= carEndVelocityY ||
					carSprite.Delta < 1 && carVelocityY <= carEndVelocityY)
				{
					if (carSprite.CarAction == CarAction.Start)
					{
						carSprite.CarAction = CarAction.Timer;
					}
					else
					{
						carSprite.CarAction = CarAction.None;
						carSprite.Velocity = carSprite.CarVelocity;
					}
				}
			}
			else
			{
				carSprite.Timer += gameTime.ElapsedGameTime.Milliseconds;
				if (carSprite.Timer > 3000)
				{
					carSprite.CarAction = CarAction.Finish;
					carSprite.Delta = 1 / carSprite.Delta;
					carSprite.EndVelocity = carSprite.CarVelocity;
				}
			}
		}

		protected void UpdateCarsPosition(CarSprite carSprite, GameTime gameTime)
		{
			if (carSprite.CarSwerve != CarSwerve.Timer)
			{
				carSprite.Movement++;
				if (carSprite.Movement > 15 * game.DeviceFactory.Scale)
				{
					carSprite.Velocity = new Vector2(0, carSprite.Velocity.Y);
					if (carSprite.CarSwerve == CarSwerve.Start)
					{
						carSprite.CarSwerve = CarSwerve.Timer;
					}
					else
					{
						carSprite.CarSwerve = CarSwerve.None;

						Vector2 position = carSprite.Position;
						position.X = carSprite.EndPositionX;
						carSprite.Position = position;
					}
				}
			}
			else
			{
				carSprite.Timer += gameTime.ElapsedGameTime.Milliseconds;
				if (carSprite.Timer > 3000)
				{
					carSprite.CarSwerve = CarSwerve.Finish;
					carSprite.Delta *= -1;

					carSprite.Velocity = new Vector2(carSprite.Delta, carSprite.Velocity.Y);
					carSprite.Movement = 0;
				}
			}
		}

		protected void UpdateGoalLabel(GameTime gameTime)
		{
			gameVariable.GoalTimer += gameTime.ElapsedGameTime.Milliseconds;
			if (gameVariable.GoalTimer > 800)
			{
				gameVariable.GoalTimer = 0;
				gameVariable.GoalDisplay = !gameVariable.GoalDisplay;
			}
		}

		private void UnlockGame()
		{
			// Cannot unlock game while asking to quit.
			if (gameVariable.MasterQuit)
			{
				return;
			}

			// Cannot unlock game if playing anonymously.
			if (gameVariable.SignedInGamer == null)
			{
				return;
			}

			// Cannot unlock game if guide is visible.
			if (Guide.IsVisible)
			{
				return;
			}

			// Unlock detection.
			Boolean unlock = inputState.Unlock();
			if (!unlock)
			{
				return;
			}

			// Cannot unlock game if player is not signed into Live.
			if (!gameVariable.SignedInGamer.IsSignedInToLive)
			{
				// ERROR!!
				InitializeQuestion("Player not signed into LIVE!", game.DeviceFactory.ContText, String.Empty);
				gameVariable.UnlockError = true;
				gameVariable.MasterQuit = true;
				return;
			}

			// Cannot unlock game if player not allowed to purchase content.
			if (!gameVariable.SignedInGamer.Privileges.AllowPurchaseContent)
			{
				// ERROR!!
				InitializeQuestion("Player not allowed to unlock game!", game.DeviceFactory.ContText, String.Empty);
				gameVariable.UnlockError = true;
				gameVariable.MasterQuit = true;
				return;
			}

			// Prompt player to unlock game.
			PlayerIndex playerIndex = gameVariable.SignedInGamer.PlayerIndex;
			game.UnlockGame(playerIndex);
		}

		protected void ResetQuestion()
		{
			gameVariable.MasterQuit = !gameVariable.MasterQuit;
			gameVariable.UnlockError = false;

			InitializeQuestion("Are you sure you want to quit?", game.DeviceFactory.NextText, game.DeviceFactory.BackText);
		}

		protected void UpdateBoost()
		{
			Single value = gameVariable.BoostDelta / scale;
			gameVariable.BoostValue -= value;

			if (gameVariable.BoostValue <= 0)
			{
				// No more boost left!
				gameVariable.BoostValue = 0;
				const String text = "No Boost";
				Vector2 measureString = gameVariable.BoostLabel.GetPosition(text);

				Single newScale2 = scale * 4;
				Single offsetX = gameWindowX + gameWindowWidth - newScale2 - measureString.X;
				Single offsetY = gameWindowY + gameWindowHeight - newScale2 - measureString.Y;
				gameVariable.BoostLabel.SetPosition(offsetX, offsetY);
			}
			else
			{
				Single delta = gameVariable.BoostDelta;

				Single boostX = gameVariable.BoostVector4.X;
				Single boostY = gameVariable.BoostVector4.Y + delta;
				Single boostWidth = gameVariable.BoostVector4.Z;
				Single boostHeight = gameVariable.BoostVector4.W - delta;

				UpdateBoostRectangle(boostX, boostY, boostWidth, boostHeight);
			}
		}

		protected void UpdateSoundEffectVolume()
		{
			Single volume = gameVariable.CarScreechEffectInstance.Volume;
			volume = volume - 0.02f;
			if (volume <= 0)
			{
				StopSoundEffectVolume();
				return;
			}

			for (Int32 index = 0; index < 2; index++)
			{
				game.HenwayGameVariable.CarEngineEffectInstance[index].Volume = volume;
				game.HenwayGameVariable.CarHornEffectInstance[index].Volume = volume;
				game.HenwayGameVariable.CarRevEffectInstance[index].Volume = volume;
			}

			game.HenwayGameVariable.CarEngineEffectInstance[2].Volume = volume;
			game.HenwayGameVariable.CarScreechEffectInstance.Volume = volume;

			if (gameVariable.SquawkEffectInstance.State == SoundState.Playing)
			{
				gameVariable.SquawkEffectInstance.Stop();
			}
		}

		private void StopSoundEffectVolume()
		{
			for (Int32 index = 0; index < 2; index++)
			{
				game.HenwayGameVariable.CarEngineEffectInstance[index].Stop();
				game.HenwayGameVariable.CarHornEffectInstance[index].Stop();
				game.HenwayGameVariable.CarRevEffectInstance[index].Stop();
			}

			game.HenwayGameVariable.CarEngineEffectInstance[2].Stop();
			game.HenwayGameVariable.CarScreechEffectInstance.Stop();
			game.HenwayGameVariable.SquawkEffectInstance.Stop();
		}

		protected void UpdateBoostRectangle(Single boostX, Single boostY, Single boostWidth, Single boostHeight)
		{
			Single space = scale * 2;
			Vector4 boostVector4 = new Vector4(
				boostX,
				boostY,
				boostWidth,
				boostHeight
				);

			gameVariable.BoostVector4 = boostVector4;
			gameVariable.BoostRectangle = ComputeBoostRectangle(gameVariable.BoostVector4, gameVariable.BoostRectangle);

			Vector4 boostVector4Back = new Vector4(
				boostX - space,
				boostY - space,
				boostWidth + 2 * space,
				boostHeight + 2 * space
				);

			gameVariable.BoostVector4Back = boostVector4Back;
			gameVariable.BoostRectangleBack = ComputeBoostRectangle(gameVariable.BoostVector4Back, gameVariable.BoostRectangleBack);
		}

		private static Rectangle ComputeBoostRectangle(Vector4 oldVector4, Rectangle oldRectangle)
		{
			Int32 oldBottom = oldRectangle.Bottom;
			if (oldBottom == 0)
			{
				oldBottom = (Int32)oldVector4.Y + (Int32)oldVector4.W;
			}

			Int32 x = (Int32)oldVector4.X;
			Int32 y = (Int32)oldVector4.Y;
			Int32 width = (Int32)oldVector4.Z;
			Int32 height = (Int32)oldVector4.W;

			Int32 newBottom = height + y;
			if (newBottom != oldBottom)
			{
				height = oldBottom - y;
			}

			Rectangle newRectangle = new Rectangle(x, y, width, height);
			return newRectangle;
		}

	}
}