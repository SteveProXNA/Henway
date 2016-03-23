using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Sprite.Action;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState04Cross : AbstractGameState
	{
		private Vector2 highwayPosition;
		private Vector2 chickenSpritePosition;
		private Rectangle chickenSpriteRectangle;
		private IList<Vector2> carSpritePosition;

		private IList<Vector2> roadSignSpritePosition;
		private IList<Texture2D> roadSignSpriteTexture;
		private Int32 roadSigns;

		private Int32 count;
		private const Int32 space = 8;

		private Double timeSpan;
		private Boolean firstTime;

		public GameState04Cross(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize chicken.
			gameVariable.ChickenSprite.CurFrame = 0;
			highwayPosition = new Vector2(gameWindowX, gameWindowY);
			gameVariable.ChickenSprite.CurFrame = 0;
			chickenSpritePosition = gameVariable.ChickenSprite.Position;
			chickenSpriteRectangle = gameVariable.ChickenSprite.DrawRectangle;

			carSpritePosition = new List<Vector2>(gameVariable.MaxCars);
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				Vector2 position = gameVariable.CarSprites[index].Position;
				carSpritePosition.Add(position);
			}

			// Update chicken sprite for next road cross.
			Single positionX = 4 + gameWindowX;
			Single positionY = gameVariable.ChickenSprite.Position.Y;
			gameVariable.ChickenSprite.Position = new Vector2(positionX, positionY);
			gameVariable.ChickenSprite.CurFrame = 0;

			// Update car sprites for next road cross.
			IList<Vector2> carPositions = InitializeCarPositions();
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				gameVariable.CarSprites[index].Position = carPositions[index];
				gameVariable.CarSprites[index].Velocity = gameVariable.CarSprites[index].CarVelocity;
				gameVariable.CarSprites[index].CarAction = CarAction.None;
				gameVariable.CarSprites[index].CarSwerve = CarSwerve.None;
			}

			// Initialize road signs as necc.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				roadSigns = gameVariable.RoadSignList.Count;
				roadSignSpritePosition = new List<Vector2>(roadSigns);
				roadSignSpriteTexture = new List<Texture2D>(roadSigns);

				for (Int32 index = 0; index < roadSigns; index++)
				{
					Vector2 position = gameVariable.RoadSignList[index].Position;
					Texture2D texture = gameVariable.RoadSignList[index].Texture;

					roadSignSpritePosition.Add(position);
					roadSignSpriteTexture.Add(texture);
				}
			}

			// Initialize level and road label.
			InitializeLevelLabels();
			count = 0;

			firstTime = true;
			timeSpan = 0;
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
					gameVariable.NextGameState = EnumGameState.GameState03Play;
				}
			}
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSpan > 500)
			{
				if (firstTime)
				{
					firstTime = false;
				}
			}

			// Update SoundEffectVolume.
			UpdateSoundEffectVolume();

			// Update highway position.
			highwayPosition.X -= space;

			// Update chicken position.
			if (chickenSpritePosition.X > 4 + gameWindowX)
			{
				chickenSpritePosition.X -= space;
			}
			else
			{
				chickenSpritePosition.X = gameWindowX + 4;
			}

			// Update car position.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				Int32 newX = (Int32)carSpritePosition[index].X - space;
				Int32 newY = (Int32)carSpritePosition[index].Y;

				carSpritePosition[index] = new Vector2(newX, newY);
			}

			// Update road signs position as necc.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				for (Int32 index = 0; index < roadSigns; index++)
				{
					Int32 newX = (Int32)roadSignSpritePosition[index].X - space;
					Int32 newY = (Int32)roadSignSpritePosition[index].Y;

					roadSignSpritePosition[index] = new Vector2(newX, newY);
				}
			}

			// Check if scrolling complete.
			count += space;
			if (count >= gameWindowWidth)
			{
				gameVariable.NextGameState = EnumGameState.GameState03Play;
			}
		}

		public override void DrawHighway()
		{
			// Draw highway texture.
			spriteBatch.Draw(gameVariable.HighwayTexture, highwayPosition, Color.White);
			spriteBatch.Draw(gameVariable.HighwayTexture, new Vector2(highwayPosition.X + gameWindowWidth, highwayPosition.Y), Color.White);
		}

		public override void DrawChicken()
		{
			// Draw chicken.
			spriteBatch.Draw(gameVariable.ChickenTexture, chickenSpritePosition, chickenSpriteRectangle, Color.White);
		}

		public override void DrawCars()
		{
			// Draw road signs as necc.
			if (game.DeviceFactory.RoadSignPosn != null)
			{
				Int32 space = (Int32)(game.DeviceFactory.Scale * 2);
				for (Int32 index = 0; index < roadSigns; index++)
				{
					Vector2 position = roadSignSpritePosition[index];
					Texture2D texture = roadSignSpriteTexture[index];

					Rectangle borderRectangle = new Rectangle(
						(Int32)position.X - space,
						(Int32)position.Y - space,
						texture.Width + 2 * space,
						texture.Height + 2 * space
						);

					// Draw first road sign.
					spriteBatch.Draw(texture, borderRectangle, Color.Black);
					spriteBatch.Draw(texture, position, Color.White);

					Single newPositionX = position.X + gameWindowWidth;
					position = new Vector2(newPositionX, position.Y);
					borderRectangle = new Rectangle(
						(Int32)newPositionX - space,
						(Int32)position.Y - space,
						texture.Width + 2 * space,
						texture.Height + 2 * space
						);

					// Draw second road sign.
					spriteBatch.Draw(texture, borderRectangle, Color.Black);
					spriteBatch.Draw(texture, position, Color.White);
				}
			}

			// Draw cars.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				spriteBatch.Draw(gameVariable.CarTexture[index], carSpritePosition[index], Color.White);
			}
		}

		public override void DrawHud()
		{
			// Draw chicken head sprites.
			DrawChickenHeadCommon();

			// Draw level, road and high label.
			DrawLevelRoadHighCommon();

			// Draw boost gauge.
			DrawBoost();

			// Draw quit question if applicable.
			base.DrawHud();
		}

	}
}