using System;
using Microsoft.Xna.Framework;
using Henway.Sprite.Action;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState05Finish : AbstractGameState
	{
		private Double timeSpan;
		private Boolean firstTime;
		private Boolean brokenRecord;

		private Int32 chickenComplete;
		private Int32 chickenCurFrame;
		private Int32 chickenUpdFrame;

		public GameState05Finish(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			// Initialize finish sequence.
			gameVariable.CelebrateEffectInstance.Play();

			// Initialize level number label.
			InitializeLevelNumberLabel();

			// Initialize level and road label.
			InitializeLevelLabels();

			// Initialize chicken.
			chickenComplete = 0;
			chickenCurFrame = 0;
			chickenUpdFrame = 0;
			gameVariable.ChickenSprite.CurFrame = chickenCurFrame;

			// Initialize cars.
			for (Int32 index = 0; index < gameVariable.MaxCars; index++)
			{
				gameVariable.CarSprites[index].BoundsAction = BoundsAction.Finish;
				gameVariable.CarSprites[index].CarAction = CarAction.None;
			}

			// Increase game difficulty.  Check if Player has broken record.
			brokenRecord = false;
			gameVariable.CarLevel++;
			if (gameVariable.CarLevel > gameVariable.MaxLevel)
			{
				brokenRecord = true;

				String text = game.DeviceFactory.RecordText;
				Vector2 measureString = gameVariable.RecordBreakLabel.GetPosition(text);

				Single newScale = scale * 4;
				Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
				Single offsetY = gameWindowY + gameWindowHeight - newScale - measureString.Y;
				gameVariable.RecordBreakLabel.SetPosition(offsetX, offsetY);
			}

			timeSpan = 0;
			firstTime = true;
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
						AdvanceLevel(EnumGameState.GameState01Title);
					}
					else
					{
						ResetQuestion();
					}
				}
				else
				{
					AdvanceLevel(EnumGameState.GameState02Level);
				}
			}
		}

		public override void UpdateSprites(GameTime gameTime)
		{
			// Update chicken.
			if (chickenComplete < 2)
			{
				chickenUpdFrame++;
				Int32 cycle = (Int32)(6 * scale);
				if (chickenUpdFrame % cycle == 0)
				{
					chickenUpdFrame = 0;
					chickenCurFrame += 1;
					if (chickenCurFrame >= gameVariable.ChickenSprite.NumFrame)
					{
						chickenCurFrame = 0;
						chickenComplete++;
					}

					gameVariable.ChickenSprite.CurFrame = chickenCurFrame;
				}
			}

			// Update car sprites.
			UpdateCarsCommon(gameTime);
		}

		public override void UpdateGeneral(GameTime gameTime)
		{
			// Update SoundEffectVolume.
			UpdateSoundEffectVolume();

			timeSpan += gameTime.ElapsedGameTime.Milliseconds;
			if (timeSpan > 500)
			{
				if (firstTime)
				{
					firstTime = false;
				}
			}
			if (timeSpan > gameVariable.TimeConstant)
			{
				AdvanceLevel(EnumGameState.GameState02Level);
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
			// Draw chicken head sprites.
			DrawChickenHeadCommon();

			// Draw level, road and high label.
			DrawLevelRoadHighCommon();

			// Draw boost gauge.
			DrawBoost();

			// Draw level.
			if (!gameVariable.MasterQuit)
			{
				gameVariable.TitleLabel[0].Draw();
				gameVariable.TitleLabel[1].Draw();
			}

			// Draw broken record.
			if (brokenRecord)
			{
				gameVariable.RecordBreakLabel.Draw();
			}

			// Draw quit question if applicable.
			base.DrawHud();
		}

		private void AdvanceLevel(EnumGameState saveEnumGameState)
		{
			gameVariable.NextGameState = saveEnumGameState;
			if (brokenRecord)
			{
				gameVariable.NextGameState = game.DeviceFactory.SaveStorageState(saveEnumGameState);
				game.DeviceFactory.SaveStorage();
			}
		}

	}
}