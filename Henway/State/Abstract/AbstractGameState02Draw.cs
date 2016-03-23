using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Henway.State.Abstract
{
	/// <summary>
	/// Template method implementations for Draw.
	/// </summary>
	public abstract partial class AbstractGameState
	{
		public virtual void DrawHighway()
		{
			// Draw highway texture.
			Vector2 highwayPosition = new Vector2(gameWindowX, gameWindowY);
			spriteBatch.Draw(gameVariable.HighwayTexture, highwayPosition, Color.White);
		}

		public virtual void DrawChicken()
		{
		}

		public virtual void DrawCars()
		{
		}

		public virtual void DrawHud()
		{
			// Draw quit question if applicable.
			if (!gameVariable.MasterQuit)
			{
				return;
			}

			spriteBatch.Draw(gameVariable.BlankTexture, gameVariable.BlankRectangle, gameVariable.BlankColor);
			for (Int32 index = 0; index < 3; index++)
			{
				gameVariable.QuitLabel[index].Draw();
			}
		}

		// Common draw methods.
		protected void DrawChickenCommon()
		{
			// Draw chicken sprite.
			gameVariable.ChickenSprite.Draw(spriteBatch);
		}

		protected void DrawCarsCommon()
		{
			// Draw car sprites.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				gameVariable.CarSprites[index].Draw(spriteBatch);
			}
		}

		protected void DrawRoadSignCommon()
		{
			if (game.DeviceFactory.RoadSignPosn == null)
			{
				return;
			}
			if (gameVariable.RoadSignList == null)
			{
				return;
			}

			// Draw road signs as necc.
			for (Int32 index = 0; index < gameVariable.RoadSignList.Count; index++)
			{
				gameVariable.RoadSignList[index].Draw(spriteBatch);
			}
		}

		protected void DrawChickenHeadCommon()
		{
			// Draw chicken head sprites.
			Single chickenHeadX = gameVariable.ChickenHeadPosition.X;
			Single chickenHeadY = gameVariable.ChickenHeadPosition.Y;

			for (Int32 chickenHead = 0; chickenHead < gameVariable.NumberLives; chickenHead++)
			{
				Vector2 chickenHeadPosition = new Vector2(chickenHeadX, chickenHeadY);
				spriteBatch.Draw(gameVariable.ChickenHeadTexture, chickenHeadPosition, Color.White);

				chickenHeadX += 20 * scale;
			}
		}

		protected void DrawLevelRoadHighCommon()
		{
			// Draw level label.
			gameVariable.LevelLabel[0].Draw();
			gameVariable.LevelLabel[1].Draw();

			// Draw road label.
			gameVariable.RoadLabel[0].Draw();
			gameVariable.RoadLabel[1].Draw();

			// Draw high label.
			gameVariable.HighLabel.Draw();
		}

		protected void DrawGoalLabel()
		{
			for (Int32 index = 0; index < gameVariable.GoalLabel.Length; index++)
			{
				gameVariable.GoalLabel[index].Draw();
			}
		}

		protected void DrawBoost()
		{
			if (gameVariable.BoostValue > 0)
			{
				game.SpriteBatch.Draw(gameVariable.BoostTexture, gameVariable.BoostRectangleBack, Color.Black);
				game.SpriteBatch.Draw(gameVariable.BoostTexture, gameVariable.BoostRectangle, Color.White);
			}

			gameVariable.BoostLabel.Draw();
		}

	}
}