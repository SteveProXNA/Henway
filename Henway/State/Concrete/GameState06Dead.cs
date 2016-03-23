using System;
using Henway.Sprite;
using Henway.Sprite.Action;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState06Dead : AbstractGameState
	{
		private Boolean displayChicken;
		private Boolean firstTime;
		private Double deadSpan;
		private Double timeSpan;
		private Single delta;

		private DeathState deathState;
		private enum DeathState
		{
			Dying,
			Dead,
			Bury
		};

		public GameState06Dead(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize death sequence.
			gameVariable.SquishEffectInstance.Play();
			gameVariable.NumberLives--;

			displayChicken = true;
			firstTime = true;
			deathState = DeathState.Dying;
			deadSpan = 0;
			timeSpan = 0;

			// Rumble controller on death.
			inputState.SetMotors(1, 0);
			delta = CalculateDelta();
		}

		public override void UpdateInput(GameTime gameTime)
		{
			if (firstTime)
			{
				return;
			}

			base.UpdateInput(gameTime);

			// Check if player cancels quit.
			CheckCancelQuit();

			// Check if player accepts quit.
			Boolean next = inputState.Next();
			if (next)
			{
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
					DetermineNextState();
				}
			}
		}

		public override void UpdateSprites(GameTime gameTime)
		{
			// Update car sprites.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				CarSprite carSprite = gameVariable.CarSprites[index];
				if (index == gameVariable.DeathCar)
				{
					Single velocityY = carSprite.Velocity.Y;
					if (delta == 0)
					{
						velocityY = 0;
					}
					else
					{
						velocityY /= delta;
					}

					if (Math.Abs(velocityY) <= 0)
					{
						velocityY = 0;
					}

					carSprite.Velocity = new Vector2(carSprite.Velocity.X, velocityY);
				}

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

		public override void UpdateGeneral(GameTime gameTime)
		{
			// Update goal timer to flash goal text.
			UpdateGoalLabel(gameTime);

			// Update SoundEffectVolume.
			UpdateSoundEffectVolume();

			// Bury.
			if (deathState == DeathState.Bury)
			{
				DetermineNextState();
				return;
			}

			// Dead.
			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (deathState == DeathState.Dead)
			{
				if (timeSpan > gameVariable.TimeConstant)
				{
					deathState = DeathState.Bury;
					return;
				}
			}
			else
			{
				// Dying.
				if (timeSpan > gameVariable.TimeConstant / 2)
				{
					timeSpan = 0;
					deathState = DeathState.Dead;
					return;
				}

				deadSpan += gameTime.ElapsedGameTime.Milliseconds;
				if (deadSpan > 500)
				{
					deadSpan = 0;
					displayChicken = !displayChicken;

					if (firstTime)
					{
						firstTime = false;

						// Reset game controller motors.
						inputState.ResetMotor();
					}
				}
			}
		}

		public override void DrawChicken()
		{
			// If we advance to next state then do not draw chicken.
			if (gameVariable.NextGameState != EnumGameState.GameState06Dead)
			{
				return;
			}

			// Draw chicken sprite.
			if (deathState == DeathState.Dying)
			{
				if (displayChicken)
				{
					DrawChickenCommon();
				}
			}

			// Draw skull n' crossbones.
			if (deathState == DeathState.Dead)
			{
				game.SpriteBatch.Draw(gameVariable.DeathTexture, gameVariable.ChickenSprite.Position, Color.White);
			}
		}

		public override void DrawCars()
		{
			// Draw road signs as necc.
			DrawRoadSignCommon();

			// Draw car sprites.
			DrawCarsCommon();
		}

		public override void DrawHud()
		{
			// Draw chicken head sprites.
			DrawChickenHeadCommon();

			// Draw level, road and high label.
			DrawLevelRoadHighCommon();

			// Draw goal label.
			if (gameVariable.GoalDisplay)
			{
				DrawGoalLabel();
			}

			// Draw boost gauge if and only if the next state is not game over.
			if (gameVariable.NextGameState != EnumGameState.GameState07Over)
			{
				DrawBoost();
			}

			// Draw quit question if applicable.
			base.DrawHud();
		}

		private void DetermineNextState()
		{
			// Initialize Boost.
			InitializeBoost();

			// Re-instate velocity of death car.
			Int32 index = gameVariable.DeathCar;
			Vector2 velocity = gameVariable.DeathVelocity;

			gameVariable.CarSprites[index].Velocity = velocity;
			gameVariable.DeathCar = -1;

			// Initialize chicken back to start position.
			InitializeChickenCommon();

			// Determine next state.
			if (gameVariable.NumberLives <= 0)
			{
				gameVariable.NextGameState = EnumGameState.GameState07Over;
			}
			else
			{
				gameVariable.NextGameState = EnumGameState.GameState03Play;
			}
		}

		private Single CalculateDelta()
		{
			// If chicken passed car up/down the stop car immediately.
			CarDirection deathDirection = gameVariable.DeathSprite.CarDirection;
			if (deathDirection == CarDirection.Down)
			{
				if (gameVariable.ChickenSprite.Position.Y <= gameVariable.DeathSprite.Position.Y)
				{
					return 0;
				}
			}
			if (deathDirection == CarDirection.Up)
			{
				if (gameVariable.ChickenSprite.Position.Y + gameVariable.ChickenSprite.Rectangle.Height >=
					gameVariable.DeathSprite.Position.Y + gameVariable.DeathSprite.Height)
				{
					return 0;
				}
			}

			// Chicken not passed so death car will decelerate.
			Single deathVelocity = Math.Abs(gameVariable.DeathSprite.Velocity.Y);
			if (deathVelocity < 4)
			{
				// Decelerate slowly.
				return 1.05f;
			}

			// Decelerate faster.
			return 1.1f;
		}

	}
}