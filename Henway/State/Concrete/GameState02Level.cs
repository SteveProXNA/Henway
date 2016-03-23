using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Henway.State.Abstract;
using Henway.Sprite;

namespace Henway.State.Concrete
{
	public class GameState02Level : AbstractGameState
	{
		private Double timeSpan;

		public GameState02Level(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize level number label.
			InitializeLevelNumberLabel();

			// Initialize chicken.
			InitializeChickenCommon();

			// Initialize cars.
			InitializeCarsCommon();

			// Initialize other level variables.
			Int32 actionSwerve = (15 - gameVariable.CarLevel) * 30 + 100;
			if (actionSwerve < 100)
			{
				actionSwerve = 100;
			}
			else if (actionSwerve > 400)
			{
				actionSwerve = 400;
			}

			gameVariable.ActionSwerve = actionSwerve;

			// Initialize goal label.
			InitializeGoalLabel();
			gameVariable.GoalDisplay = true;
			gameVariable.GoalTimer = 0;

			// Initialize Boost.
			InitializeBoost();
			Single deltaDenominator = 1.0f;
			if (gameVariable.CarLevel < 5)
			{
				deltaDenominator = 2.0f;
			}
			if (gameVariable.CarLevel < 3)
			{
				deltaDenominator = 3.0f;
			}
			deltaDenominator += 0;
			gameVariable.BoostDelta = 1.0f / deltaDenominator;

			// Initialize road sign.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				InitializeRoadSign();
			}

			gameVariable.RoadComplete = 0;
			gameVariable.RoadPercent = 0;
			timeSpan = 0;
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
				else
				{
					SetNextState();
				}
			}
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSpan > gameVariable.TimeConstant)
			{
				SetNextState();
			}
		}

		public override void DrawHud()
		{
			// Draw level.
			if (!gameVariable.MasterQuit)
			{
				gameVariable.TitleLabel[0].Draw();
				gameVariable.TitleLabel[1].Draw();

				gameVariable.HintLevelLabel.Draw();
			}

			// Draw quit question if applicable.
			base.DrawHud();
		}

		private void SetNextState()
		{
			gameVariable.NextGameState = EnumGameState.GameState03Play;

			// Initialize level with car engines.
			gameVariable.CarEngineEffectInstance[0].Volume = game.DeviceFactory.SoundEffectVolume;
			if (gameVariable.CarEngineEffectInstance[0].State != SoundState.Playing)
			{
				gameVariable.CarEngineEffectInstance[0].Play();
			}
		}

		private void InitializeRoadSign()
		{
			gameVariable.RoadSignList.Clear();
			gameVariable.RoadSignDictionary.Clear();

			// Begin road signs from third level on.
			Int32 tempLevel = gameVariable.CarLevel;
			if (game.DeviceFactory.RoadSignLocn.Length == 0)
			{
				return;
			}

			// Road signs remain same after all levels exhausted.
			if (tempLevel > game.DeviceFactory.RoadSignLocn.Length)
			{
				tempLevel = game.DeviceFactory.RoadSignLocn.Length;
			}

			Int32 roadLevel = tempLevel - 1;
			String roadSignLocn = game.DeviceFactory.RoadSignLocn[roadLevel];
			if (roadSignLocn.Length == 0)
			{
				return;
			}

			String roadSignSize = game.DeviceFactory.RoadSignSize[roadLevel];

			Char[] charSignLocn = roadSignLocn.ToCharArray();
			Char[] charSignSize = roadSignSize.ToCharArray();
			Int32 randomOrder = gameVariable.Random.Next(2);

			for (Int32 index = 0; index < charSignLocn.Length; index++)
			{
				// Location.
				Char cSignLocn = charSignLocn[index];
				String sSignLocn = cSignLocn.ToString();
				Int32 iSignLocn = Convert.ToInt32(sSignLocn);

				// Size.
				Char cSignSize = charSignSize[index];
				String sSignSize = cSignSize.ToString();
				Int32 iSignSize = Convert.ToInt32(sSignSize);

				// Get road sign.
				Int32 iRoadSignSprite = iSignLocn - 1;
				RoadSignSprite tempRoadSignSprite = gameVariable.RoadSignMaster[iSignLocn - 1];

				// Mod road sign size.
				Texture2D texture = gameVariable.RoadSignTexture[iSignSize];
				if (tempRoadSignSprite.Texture.Height != texture.Height)
				{
					tempRoadSignSprite.UpdateTexture(texture);
				}

				// High.
				Char cSignHigh = game.DeviceFactory.RoadSignHigh[iRoadSignSprite];
				Single newX = tempRoadSignSprite.Position.X;
				Single newY = GetRoadSignSpriteY(cSignHigh, iSignSize, randomOrder);

				Vector2 newPosition = new Vector2(newX, newY);
				tempRoadSignSprite.Position = newPosition;
				tempRoadSignSprite.UpdateCollision();

				// Update algorithm to load road sign list and add to dictionary.
				gameVariable.RoadSignList.Add(tempRoadSignSprite);

				// Add collision to dictionary.
				Int32 left = iSignLocn + 20;
				Int32 rght = iSignLocn + 11;

				gameVariable.RoadSignDictionary[left] = index;
				gameVariable.RoadSignDictionary[rght] = index;
			}
		}

		private Single GetRoadSignSpriteY(Char cSignHigh, Int32 iSignSize, Int32 randomOrder)
		{
			Single newY = 0;

			// Swap.
			if (cSignHigh != 'M')
			{
				if (randomOrder == 1)
				{
					if (cSignHigh == 'T')
					{
						cSignHigh = 'B';
					}
					else if (cSignHigh == 'B')
					{
						cSignHigh = 'T';
					}
				}
			}

			if (iSignSize == 0)
			{
				// Large.
				switch (cSignHigh)
				{
					case 'T':
						newY = 0;
						break;
					case 'M':
						newY = 180;
						break;
					case 'B':
						newY = 360;
						break;
				}
			}
			else if (iSignSize == 1)
			{
				// Small.
				Int32 max = 54;
				Int32 str = 0;
				switch (cSignHigh)
				{
					case 'T':
						str = 72;
						break;
					case 'M':
						str = 180;
						max = 108;
						break;
					case 'B':
						str = 342;
						break;
				}

				Int32 rnd = gameVariable.Random.Next(max);
				newY = str + rnd;
			}

			newY += gameWindowY;
			return newY;
		}

	}
}