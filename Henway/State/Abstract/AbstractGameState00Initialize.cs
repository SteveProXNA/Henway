using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Henway.Sprite.Action;

namespace Henway.State.Abstract
{
	public abstract partial class AbstractGameState
	{
		protected void InitializeChickenCommon()
		{
			// Initialize chicken.
			Single positionX = 4 + gameWindowX;
			Single positionY = gameWindowY + ((gameWindowHeight - gameVariable.ChickenSprite.Texture.Height) / 2);
			gameVariable.ChickenSprite.Position = new Vector2(positionX, positionY);
		}

		protected void InitializeCarsCommon()
		{
			// Initialize car direction.
			InitializeCarDirections();

			// Initialize car positions.
			IList<Vector2> carPositions = InitializeCarPositions();

			// Initialize car velocities.
			IList<Vector2> carVelocitys = InitializeCarVelocitys();

			// Initialize cars.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				gameVariable.CarSprites[index].BoundsAction = BoundsAction.Wrap;
				gameVariable.CarSprites[index].CarAction = CarAction.None;
				gameVariable.CarSprites[index].CarSwerve = CarSwerve.None;

				gameVariable.CarSprites[index].Position = carPositions[index];
				gameVariable.CarSprites[index].Velocity = carVelocitys[index];
				gameVariable.CarSprites[index].CarVelocity = carVelocitys[index];
			}
		}

		protected void InitializeCarDirections()
		{
			Int32 carLevel = gameVariable.CarLevel % 3;
			Int32 roads = gameVariable.MaxCars / 4;

			// Forwards.
			if (carLevel == 1)
			{
				for (Int32 cnt = 0; cnt < roads; cnt++)
				{
					Int32 idx = cnt * 4;
					gameVariable.CarSprites[idx + 0].CarDirection = gameVariable.CarSprites[idx + 1].CarDirection = CarDirection.Down;
					gameVariable.CarSprites[idx + 2].CarDirection = gameVariable.CarSprites[idx + 3].CarDirection = CarDirection.Up;
				}

				return;
			}

			// Backwards.
			if (carLevel == 2)
			{
				for (Int32 cnt = 0; cnt < roads; cnt++)
				{
					Int32 idx = cnt * 4;
					gameVariable.CarSprites[idx + 0].CarDirection = gameVariable.CarSprites[idx + 1].CarDirection = CarDirection.Up;
					gameVariable.CarSprites[idx + 2].CarDirection = gameVariable.CarSprites[idx + 3].CarDirection = CarDirection.Down;
				}

				return;
			}

			// Anarchy.
			for (Int32 cnt = 0; cnt < roads; cnt++)
			{
				Int32 idx = cnt * 4;
				gameVariable.CarSprites[idx + 0].CarDirection = DirectionRandom();
				gameVariable.CarSprites[idx + 1].CarDirection = DirectionOpposite(idx + 0);
				gameVariable.CarSprites[idx + 2].CarDirection = DirectionRandom();
				gameVariable.CarSprites[idx + 3].CarDirection = DirectionOpposite(idx + 2);
			}
		}

		protected IList<Vector2> InitializeCarPositions()
		{
			// Initialize car positions.
			IList<Vector2> carPositions = new List<Vector2>();
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				CarDirection carDirection = gameVariable.CarSprites[index].CarDirection;
				Single height = -game.HenwayGameVariable.CarSprites[index].Texture.Height;
				if (carDirection == CarDirection.Up)
				{
					height = gameWindowY + gameWindowHeight;
				}

				Single width = game.DeviceFactory.CarLeftStart[index];
				width += gameWindowX;
				Vector2 carPosition = new Vector2(width, height);
				carPositions.Add(carPosition);
			}

			return carPositions;
		}

		protected IList<Vector2> InitializeCarVelocitys()
		{
			// Determine min and max velocity based on car level.
			Int32 tempLevel = gameVariable.CarLevel;
			if (tempLevel > gameVariable.MinValue.Length)
			{
				tempLevel = gameVariable.MinValue.Length;
			}

			Int32 fastLevel = tempLevel - 1;

			Int32 minCarSpeed = gameVariable.MinValue[fastLevel];
			Int32 maxCarSpeed = gameVariable.MaxValue[fastLevel];

			// Initialize car velocities.
			IList<Vector2> carVelocitys = new List<Vector2>();
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				Single currCarSpeed = gameVariable.Random.Next(minCarSpeed, maxCarSpeed + 1);

				Single multiplier;
				while (true)
				{
					multiplier = (Single)gameVariable.Random.NextDouble();
					if (multiplier > 0.95)
					{
						break;
					}
				}

				currCarSpeed *= multiplier;

				CarDirection carDirection = gameVariable.CarSprites[index].CarDirection;
				currCarSpeed *= (Int32)carDirection;

				Vector2 carVelocity = new Vector2(0, currCarSpeed);
				carVelocitys.Add(carVelocity);
			}

			return carVelocitys;
		}

		protected void InitializeHenwayLabel()
		{
			// Initialize game label.
			const String text = "HENWAY";
			Vector2 measureString = gameVariable.HenwayLabel.GetPosition(text);

			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = gameWindowY + (gameWindowHeight - measureString.Y) / 4;
			gameVariable.HenwayLabel.SetPosition(offsetX, offsetY);
		}

		protected void InitializeLevelNumberLabel()
		{
			// Initialize level number label.
			String text = "LEVEL : " + gameVariable.CarLevel;
			Vector2 measureString = gameVariable.TitleLabel[0].GetPosition(text);

			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = (Single)(gameWindowY + (gameWindowHeight - measureString.Y) / 2.25);
			gameVariable.TitleLabel[0].SetPosition(offsetX, offsetY);

			String levelText;
			String hintText = String.Empty;
			if (gameVariable.CurrGameState == EnumGameState.GameState02Level)
			{
				Int32 carLevel = gameVariable.CarLevel % 3;
				switch (carLevel)
				{
					case 1:
						levelText = "Forwards";
						hintText = game.DeviceFactory.HintText[0];
						break;
					case 2:
						levelText = "Backwards";
						hintText = game.DeviceFactory.HintText[1];
						break;
					default:
						levelText = "Anarchy!";
						hintText = game.DeviceFactory.HintText[2];
						break;
				}
			}
			else
			{
				levelText = "COMPLETE";
			}

			measureString = gameVariable.TitleLabel[1].GetPosition(levelText);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 64;
			gameVariable.TitleLabel[1].SetPosition(offsetX, offsetY);

			measureString = gameVariable.HintLevelLabel.GetPosition(hintText);

			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY = gameWindowY + (gameWindowHeight - measureString.Y);
			gameVariable.HintLevelLabel.SetPosition(offsetX, offsetY);
		}

		protected void InitializeLevelLabels()
		{
			// Initialize level, road and high label.
			Single newScale = scale * 4;
			Single offsetX = gameWindowX + newScale;
			Single offsetY = gameWindowY + newScale;

			// Level label 0.
			gameVariable.LevelLabel[0].SetPosition(offsetX, offsetY);
			gameVariable.LevelLabel[0].Text = "GUEST";
			if (gameVariable.SignedInGamer != null)
			{
				gameVariable.LevelLabel[0].Text = gameVariable.SignedInGamer.Gamertag;
			}

			// Level label 1.
			String text = "Level: " + gameVariable.CarLevel;
			gameVariable.LevelLabel[1].GetPosition(text);

			offsetY += scale * 28;
			gameVariable.LevelLabel[1].SetPosition(offsetX, offsetY);

			// Chicken head position.
			offsetY += scale * 32;
			gameVariable.ChickenHeadPosition = new Vector2(offsetX, offsetY);

			// Road label 0.
			Vector2 measureString = gameVariable.RoadLabel[0].GetPosition("Progress");
			offsetX = gameWindowX + gameWindowWidth - newScale - measureString.X;
			offsetY = gameWindowY + newScale;
			gameVariable.RoadLabel[0].SetPosition(offsetX, offsetY);


			// Road label 1.
			Int32 tempCarLevel = gameVariable.CarLevel;
			if (tempCarLevel > 5)
			{
				tempCarLevel = 5;
			}

			text = String.Format("Road:{0}/{1}", gameVariable.RoadComplete, tempCarLevel);
			measureString = gameVariable.RoadLabel[1].GetPosition(text);
			offsetX = gameWindowX + gameWindowWidth - newScale - measureString.X;
			offsetY += scale * 28;
			gameVariable.RoadLabel[1].SetPosition(offsetX, offsetY);

			// High label 0.
			text = "Record: " + gameVariable.MaxLevel;
			measureString = gameVariable.HighLabel.GetPosition(text);
			offsetX = gameWindowX + newScale;
			offsetY = gameWindowY + gameWindowHeight - newScale - measureString.Y;
			gameVariable.HighLabel.SetPosition(offsetX, offsetY);
		}

		protected void InitializeQuestion(String message, String nextText, String backText)
		{
			Vector2 measureString = gameVariable.QuitLabel[0].GetPosition(message);
			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = (Single)(gameWindowY + (gameWindowHeight - measureString.Y) / 2.25);
			gameVariable.QuitLabel[0].SetPosition(offsetX, offsetY);

			measureString = gameVariable.QuitLabel[1].GetPosition(nextText);
			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 48;
			gameVariable.QuitLabel[1].SetPosition(offsetX, offsetY);
			gameVariable.QuitLabel[1].Text = nextText;

			offsetY += scale * 36;
			gameVariable.QuitLabel[2].SetPosition(offsetX, offsetY);
			gameVariable.QuitLabel[2].Text = backText;

			gameVariable.BlankRectangle = new Rectangle(
				gameWindowX,
				gameWindowY,
				gameWindowWidth,
				gameWindowHeight
				);
		}

		protected void InitializeGoalLabel()
		{
			// Goal label.
			String[] goalTexts = new String[] { "G", "O", "A", "L", "!!" };
			Vector2 measureString = gameVariable.GoalLabel[0].GetPosition(goalTexts[0]);

			Single goalSingle = measureString.Y;
			goalSingle -= (Int32)(4 * scale);
			Single goalTotal = gameVariable.GoalLabel.Length * goalSingle;

			Single offsetX = gameWindowX + gameWindowWidth - (Int32)(40 * scale);
			Single offsetY = gameWindowY + (gameWindowHeight - goalTotal) / 2;
			for (Int32 index = 0; index < gameVariable.GoalLabel.Length; index++)
			{
				gameVariable.GoalLabel[index].SetPosition(offsetX, offsetY);
				gameVariable.GoalLabel[index].Text = goalTexts[index];

				offsetY += goalSingle;
			}
		}

		protected void InitializeBoost()
		{
			gameVariable.BoostValue = 100;
			Int32 newScale = (Int32)(scale * 12);

			Single boostX = gameWindowX + gameWindowWidth - newScale - gameVariable.BoostTexture.Width;
			Single boostY = gameWindowY + gameWindowHeight - newScale - gameVariable.BoostTexture.Height;
			Single boostWidth = gameVariable.BoostTexture.Width;
			Single boostHeight = gameVariable.BoostTexture.Height;

			UpdateBoostRectangle(boostX, boostY, boostWidth, boostHeight);

			const String text = "Boost:";
			Vector2 measureString = gameVariable.BoostLabel.GetPosition(text);

			Single newScale2 = scale * 4;
			Single offsetX = boostX - newScale2 - measureString.X;
			Single offsetY = gameWindowY + gameWindowHeight - newScale2 - measureString.Y;
			gameVariable.BoostLabel.SetPosition(offsetX, offsetY);
		}

		protected void InitializeSoundEffectVolume()
		{
			for (Int32 index = 0; index < 2; index++)
			{
				game.HenwayGameVariable.CarEngineEffectInstance[index + 1].Volume = game.DeviceFactory.SoundEffectVolume;
				game.HenwayGameVariable.CarHornEffectInstance[index].Volume = game.DeviceFactory.SoundEffectVolume;
				game.HenwayGameVariable.CarRevEffectInstance[index].Volume = game.DeviceFactory.SoundEffectVolume;
			}

			game.HenwayGameVariable.CarScreechEffectInstance.Volume = game.DeviceFactory.SoundEffectVolume;
		}

		private CarDirection DirectionRandom()
		{
			Int32 random = gameVariable.Random.Next(2);
			if (random == 0)
			{
				random = -1;
			}

			CarDirection carDirection = (CarDirection)random;
			return carDirection;
		}

		private CarDirection DirectionOpposite(Int32 index)
		{
			CarDirection carDirection = CarDirection.Down;
			if (gameVariable.CarSprites[index].CarDirection == carDirection)
			{
				carDirection = CarDirection.Up;
			}

			return carDirection;
		}

	}
}