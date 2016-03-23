using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Henway.Command;
using Henway.Sprite;
using Henway.Sprite.Action;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState03Play : AbstractGameState
	{
		private MoveCommand command;
		private Int32 updateFrame;
		private Int32 chickenCurFrame;
		private Int32 chickenUpdFrame;

		private Vector2 currVelocity;
		private Vector2 prevVelocity;
		private Boolean signCollision;
		private Boolean resetMotor;

		private Int32[] carsToCheck;
		private Int32[] signToCheck;

		public GameState03Play(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize level and road label.
			InitializeLevelLabels();

			// Initialize command object.
			chickenCurFrame = 0;
			chickenUpdFrame = 0;
			command = new MoveCommand(gameVariable.ChickenSprite, Vector2.Zero, chickenCurFrame);
			command.Execute();

			currVelocity = Vector2.Zero;
			updateFrame = 0;

			carsToCheck = new Int32[2];
			signToCheck = new Int32[2];

			// Initialize SoundEffectVolume.
			InitializeSoundEffectVolume();
		}

		public override void UpdateInput(GameTime gameTime)
		{
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
			}

			// If master quit flag true then do not update sprites or general.
			if (gameVariable.MasterQuit)
			{
				return;
			}

			// Setup cars to check this frame.
			carsToCheck = DetermineCarsToCheck();

			// Increment frame counter if saving commands.
			updateFrame++;

			// Check for movement.
			const Single step = 4.0f;
			//const Single step = 1.0f;
			Single horizontal = inputState.Horizontal();
			Single velocityX = horizontal * step;

			Single vertical = inputState.Vertical();
			Single velocityY = vertical * step;

			// Check for boost.
			Int32 steve = 3;
			if (velocityX != 0 || velocityY != 0)
			{
				if (gameVariable.BoostValue > 0)
				{
					Boolean boost = inputState.Boost();
					if (boost)
					{
						velocityX *= 2.0f;
						velocityY *= 2.0f;

						steve = 2;
						UpdateBoost();
					}
				}
			}

			// Check if chicken collides with road signs as necc.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				// Check only if chicken moves first.
				if (velocityX != 0 || velocityY != 0)
				{
					// Check only if there are road signs.
					if (gameVariable.RoadSignList.Count > 0)
					{
						signCollision = RoadSignCollision(carsToCheck, velocityX, velocityY);
						if (signCollision)
						{
							velocityX = 0;
							velocityY = 0;

							inputState.SetMotors(0, 1);
							resetMotor = true;
						}
						else
						{
							ResetMotor();
						}
					}
				}
				else
				{
					ResetMotor();
				}
			}

			// Set the velocity of the chicken if necessary.
			prevVelocity = currVelocity;
			currVelocity.X = velocityX;
			currVelocity.Y = velocityY;

			if (velocityX == 0 && velocityY == 0)
			{
				if (currVelocity != prevVelocity)
				{
					command = new MoveCommand(gameVariable.ChickenSprite, Vector2.Zero, 0);
					command.Execute();

					//gameVariable.CommandsSave.Append(command);
					//gameVariable.CommandsSaveFrames.Append(updateFrame);

					updateFrame = 0;
				}

				return;
			}

			// Determine if chicken went backwards to reverse frame.
			Int32 nextFrame = 1;
			if (velocityX < 0)
			{
				nextFrame = -1;
			}

			chickenUpdFrame++;
			if (chickenUpdFrame % steve == 0)
			{
				chickenUpdFrame = 0;
				chickenCurFrame += nextFrame;

				// Determine if time to change animation frames.
				if (chickenCurFrame < 0)
				{
					chickenCurFrame = gameVariable.ChickenSprite.NumFrame - 1;
				}
				if (chickenCurFrame >= gameVariable.ChickenSprite.NumFrame)
				{
					chickenCurFrame = 0;
				}
			}

			command = new MoveCommand(gameVariable.ChickenSprite, currVelocity, chickenCurFrame);
			command.Execute();

			// Add command to list.
			//gameVariable.CommandsSave.Append(command);
			//gameVariable.CommandsSaveFrames.Append(updateFrame);

			updateFrame = 0;
		}

		public override void UpdateSprites(GameTime gameTime)
		{
			// Update car sprites.
			UpdateCarsCommon(gameTime);

			// Detect cross road.
			if (gameVariable.ChickenSprite.Position.X > game.DeviceFactory.ChickenCross)
			{
				// Detect level complete.
				if (gameVariable.RoadComplete < gameVariable.CarLevel)
				{
					gameVariable.RoadComplete++;
				}

				if (gameVariable.CarLevel > 5)
				{
					// Maximum cross the road 5x times.
					gameVariable.RoadPercent = (Single)gameVariable.RoadComplete / 5.0f;
				}
				else
				{
					gameVariable.RoadPercent = (Single)gameVariable.RoadComplete / (Single)gameVariable.CarLevel;
				}

				if (gameVariable.RoadPercent >= 1.0f)
				{
					// Level complete; however if level trial mode and level 3 then game over.
					Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
					if (isTrialMode && gameVariable.CarLevel >= 3)
					{
						gameVariable.NextGameState = EnumGameState.GameState07Over;
					}
					else
					{
						gameVariable.NextGameState = EnumGameState.GameState05Finish;
					}
				}
				else
				{
					gameVariable.NextGameState = EnumGameState.GameState04Cross;
				}
			}

			// Detect change in car action or car swerve.
			Int32 random = gameVariable.Random.Next(gameVariable.ActionSwerve);
			if (random == 0)
			{
				// Determine to implement car action or swerve.
				DetermineCarActionSwerve(carsToCheck);
			}


			// Detect offset collision.
			ChickenSprite chickenSprite = gameVariable.ChickenSprite;
			for (Int32 index = 0; index < carsToCheck.Length; index++)
			{
				Int32 value = carsToCheck[index];
				if (value < 0)
				{
					continue;
				}

				value -= 1;
				CarSprite carSprite = gameVariable.CarSprites[value];
				Boolean collision = chickenSprite.Collision.Intersects(carSprite.Collision);
				if (collision)
				{
					// Detech pixel collision.
					collision = IntersectsPixels(chickenSprite.Rectangle, chickenSprite.TextureData, carSprite.Rectangle, carSprite.TextureData);

					if (collision)
					{
						// Store velocity of Death car.
						gameVariable.DeathCar = value;
						gameVariable.DeathVelocity = carSprite.CarVelocity;
						gameVariable.DeathSprite = carSprite;
						gameVariable.ChickenSprite.CurFrame = 0;
						carSprite.CarAction = CarAction.None;

						// Transition to death state.
						gameVariable.NextGameState = EnumGameState.GameState06Dead;
						break;
					}
				}
			}
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			// Update goal timer to flash goal text.
			UpdateGoalLabel(gameTime);

			// Beep either car horn if not already playing or chicken squawk.
			Boolean soundEffectNotPlaying;
			Int32 next = gameVariable.Random.Next(250);
			if (next == 0)
			{
				soundEffectNotPlaying = GeneralEffectNotPlaying();
				if (soundEffectNotPlaying)
				{
					// Beep either car horn if not already beeping.
					Int32 general = gameVariable.Random.Next(5);
					if (general == 1)
					{
						gameVariable.SquawkEffectInstance.Play();
					}
					else
					{
						if (general > 1)
						{
							general = 1;
						}

						gameVariable.CarHornEffectInstance[general].Play();
					}

					return;
				}
			}

			// Sound car engine if not already playing.
			next = gameVariable.Random.Next(250);
			if (next >= 3)
			{
				return;
			}

			soundEffectNotPlaying = EngineEffectNotPlaying();
			if (soundEffectNotPlaying)
			{
				next += 1;
				if (next > 2)
				{
					next = 2;
				}

				gameVariable.CarEngineEffectInstance[next].Play();
				return;
			}
		}

		private Boolean EngineEffectNotPlaying()
		{
			Boolean soundEffectNotPlaying =
				gameVariable.CarEngineEffectInstance[0].State != SoundState.Playing &&
				gameVariable.CarEngineEffectInstance[1].State != SoundState.Playing &&
				gameVariable.CarEngineEffectInstance[2].State != SoundState.Playing &&
				gameVariable.CarRevEffectInstance[0].State != SoundState.Playing &&
				gameVariable.CarRevEffectInstance[1].State != SoundState.Playing &&
				gameVariable.CarScreechEffectInstance.State != SoundState.Playing;

			return soundEffectNotPlaying;
		}

		private Boolean GeneralEffectNotPlaying()
		{
			Boolean soundEffectNotPlaying =
				gameVariable.CarHornEffectInstance[0].State != SoundState.Playing &&
				gameVariable.CarHornEffectInstance[1].State != SoundState.Playing &&
				gameVariable.SquawkEffectInstance.State != SoundState.Playing;

			return soundEffectNotPlaying;
		}

		public override void DrawChicken()
		{
			// Draw chicken sprite.
			DrawChickenCommon();
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

			// Draw boost gauge.
			DrawBoost();

			// Draw quit question if applicable.
			base.DrawHud();
		}

		/// <summary>
		/// Determines if there is overlap of the non-transparent pixels
		/// between two sprites.
		/// </summary>
		/// <param name="rectangleA">Bounding rectangle of the first sprite</param>
		/// <param name="dataA">Pixel data of the first sprite</param>
		/// <param name="rectangleB">Bouding rectangle of the second sprite</param>
		/// <param name="dataB">Pixel data of the second sprite</param>
		/// <returns>True if non-transparent pixels overlap; false otherwise</returns>
		private static Boolean IntersectsPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
		{
			// Find the bounds of the rectangle intersection.
			Int32 top = Math.Max(rectangleA.Top, rectangleB.Top);
			Int32 bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
			Int32 left = Math.Max(rectangleA.Left, rectangleB.Left);
			Int32 right = Math.Min(rectangleA.Right, rectangleB.Right);

			// Check every point within the intersection bounds.
			for (Int32 y = top; y < bottom; y++)
			{
				for (Int32 x = left; x < right; x++)
				{
					// Get the color of both pixels at this point.
					Color colorA = dataA[(x - rectangleA.Left) + (y - rectangleA.Top) * rectangleA.Width];
					Color colorB = dataB[(x - rectangleB.Left) + (y - rectangleB.Top) * rectangleB.Width];

					if (colorA.A != 0 && colorB.A != 0)
					{
						// An intersection has been found.
						return true;
					}
				}
			}

			// No intersection found.
			return false;
		}

		private void DetermineCarActionSwerve(Int32[] tempCarsToCheck)
		{
			Int32 carToUse;
			Boolean carSwerve;

			// If second element is "-2" signals to use this lane.
			if (tempCarsToCheck[1] == -2)
			{
				Int32 tempValue = tempCarsToCheck[0];
				Int32 tempBound = gameVariable.MaxCars - tempValue + 1;
				Int32 maxValue = Math.Min(3, tempBound);

				Int32 random = gameVariable.Random.Next(maxValue);
				carToUse = tempValue + random;
				carSwerve = false;
			}
			else
			{
				// Otherwise randomly choose lane either left or right of chicken.
				Int32 quotient = tempCarsToCheck[0];
				Int32 remainder = tempCarsToCheck[1];

				// If chicken is far left/right then car swerve only.
				if (remainder == -1)
				{
					carToUse = quotient;
					carSwerve = true;
				}
				// Otherwise flip coin to decide car action or car swerve.
				else
				{
					Int32 random = gameVariable.Random.Next(2);
					carToUse = random == 0 ? quotient : remainder;

					Int32 maxValue = 7 - gameVariable.CarLevel;
					if (maxValue < 2)
					{
						maxValue = 2;
					}

					random = gameVariable.Random.Next(maxValue);
					if (random == 0)
					{
						carSwerve = true;
					}
					else
					{
						carSwerve = false;
					}
				}
			}

			if (carToUse == 0)
			{
				return;
			}

			// Invoke car action or swerve accordingly.
			CarSprite carSprite = gameVariable.CarSprites[carToUse - 1];
			if (carSprite.CarAction == CarAction.None && carSprite.CarSwerve == CarSwerve.None)
			{
				if (carSwerve)
				{
					InitializeCarSwerve(carSprite);
				}
				else
				{
					// Switch existing sounds off if car action.
					for (Int32 engine = 0; engine < 3; engine++)
					{
						gameVariable.CarEngineEffectInstance[engine].Stop();
					}

					InitializeCarAction(carSprite);
				}
			}
		}

		private void InitializeCarAction(CarSprite carSprite)
		{
			// Car available to action.
			carSprite.CarAction = CarAction.Start;
			carSprite.Timer = 0;

			Single endVelocityY = Math.Abs(carSprite.Velocity.Y);
			const Int32 deltaVelocity = 4;

			SoundEffectInstance soundEffectInstance;
			Int32 move = gameVariable.Random.Next(2);
			if (move == 0)
			{
				// Decelerate.
				endVelocityY -= deltaVelocity;
				if (endVelocityY < 1)
				{
					endVelocityY = 1;
				}

				carSprite.Delta = 0.95f;
				soundEffectInstance = gameVariable.CarScreechEffectInstance;
			}
			else
			{
				// Accelerate.
				endVelocityY += deltaVelocity;

				carSprite.Delta = 1 / 0.95f;
				Int32 sound = gameVariable.Random.Next(4);
				if (sound > 0)
				{
					sound = 1;
				}
				soundEffectInstance = gameVariable.CarRevEffectInstance[sound];
			}

			endVelocityY *= (Int32)carSprite.CarDirection;
			carSprite.EndVelocity = new Vector2(0, endVelocityY);

			// Set sound effect for accelerate or decelerate.
			if (soundEffectInstance.State != SoundState.Playing)
			{
				soundEffectInstance.Play();
			}
		}

		private void InitializeCarSwerve(CarSprite carSprite)
		{
			// Car available to swerve.
			carSprite.CarSwerve = CarSwerve.Start;
			carSprite.Movement = 0;
			carSprite.Timer = 0;

			Int32 sway = gameVariable.Random.Next(2);
			if (sway == 0)
			{
				sway = -1;
			}

			carSprite.Delta = sway;
			carSprite.EndPositionX = carSprite.Position.X;
			carSprite.Velocity = new Vector2(carSprite.Delta, carSprite.Velocity.Y);
		}

		private Int32[] DetermineCarsToCheck()
		{
			Int32 chickenPosition = (Int32)gameVariable.ChickenSprite.Position.X;
			chickenPosition -= gameWindowX;
			Int32 value = gameVariable.RoadMap[chickenPosition];

			if (value < 10)
			{
				// One car to check.
				carsToCheck[0] = value;
				carsToCheck[1] = -2;
			}
			else
			{
				// Two cars to check.
				value -= 10;
				Int32 quotient = value / 2;
				Int32 remainder = value % 2;
				remainder += quotient;

				if (quotient < 1)
				{
					quotient = 1;
					remainder = -1;
				}
				if (remainder > gameVariable.MaxCars)
				{
					quotient = gameVariable.MaxCars;
					remainder = -1;
				}

				carsToCheck[0] = quotient;
				carsToCheck[1] = remainder;
			}

			return carsToCheck;
		}

		private Boolean RoadSignCollision(Int32[] tempCarsToCheck, Single tempVelocityX, Single tempVelocityY)
		{
			signToCheck[1] = tempCarsToCheck[1];
			if (tempCarsToCheck[0] == 1)
			{
				// Lane 1 - RIGHT.
				signToCheck[0] = tempCarsToCheck[0] + 20;
			}
			else if (tempCarsToCheck[0] == 8)
			{
				// Lane 8 - LEFT.
				signToCheck[0] = tempCarsToCheck[0] + 10;
			}
			else if (tempCarsToCheck[1] == -2)
			{
				// Lane 2, 3, 4, 5, 6, 7 - LEFT and RIGHT.
				signToCheck[0] = tempCarsToCheck[0] + 10;
				signToCheck[1] = tempCarsToCheck[0] + 20;
			}
			else if (tempCarsToCheck[1] > 0)
			{
				// Lane 1/2, 2/3, 3/4, 4/5, 5/6, 6/7, 7/8.
				// LEFT only as RIGHT=LEFT.
				signToCheck[0] = tempCarsToCheck[0] + 20;
				signToCheck[1] = -1;
			}

			Boolean tempCollision = false;
			for (Int32 index = 0; index < signToCheck.Length; index++)
			{
				Int32 key = signToCheck[index];
				if (key < 0)
				{
					continue;
				}

				Boolean exsists = gameVariable.RoadSignDictionary.ContainsKey(key);
				if (!exsists)
				{
					continue;
				}

				key = gameVariable.RoadSignDictionary[key];

				ChickenSprite chickenSprite = gameVariable.ChickenSprite;
				Vector2 newPosition = new Vector2();
				newPosition.X = chickenSprite.Position.X + tempVelocityX;
				newPosition.Y = chickenSprite.Position.Y + tempVelocityY;

				Rectangle tempChickenRectangle = chickenSprite.Rectangle;
				Rectangle chickenCollision = new Rectangle(
					(Int32)newPosition.X,
					(Int32)newPosition.Y,
					tempChickenRectangle.Width,
					tempChickenRectangle.Height);

				RoadSignSprite roadSignSprite = gameVariable.RoadSignList[key];
				Rectangle roadSignCollision = roadSignSprite.Collision;

				tempCollision = chickenCollision.Intersects(roadSignCollision);
				if (tempCollision)
				{
					tempCollision = IntersectsPixels(chickenCollision, chickenSprite.TextureData, roadSignSprite.Rectangle, roadSignSprite.TextureData);
				}
				if (tempCollision)
				{
					break;
				}
			}

			return tempCollision;
		}

		private void ResetMotor()
		{
			if (!resetMotor)
			{
				return;
			}

			inputState.ResetMotor();
			resetMotor = false;
		}

	}
}