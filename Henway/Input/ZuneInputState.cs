using System;
using Microsoft.Xna.Framework.Input;

namespace Henway.Input
{
	public class ZuneInputState : AbstractInputState
	{
		public ZuneInputState()
		{
			ZuneController = ZuneController.RightLandscape;
			KeysDetection = false;

			// Buttons.
			ButtonsExit = new[] { Buttons.Back };
			ButtonsStart = Buttons.A;
			ButtonsPlay = Buttons.B;

			ButtonsQuit = Buttons.Back;
			ButtonsNext = Buttons.B;
			ButtonsBack = Buttons.Back;

			ButtonsBoost = Buttons.B;
			ButtonsUnlock = Buttons.X;				// Cannot unlock game on Zune!
		}

		public override Single Horizontal()
		{
			Single horizontal = 0.0f;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					horizontal = currGamePadState.ThumbSticks.Left.Y;
					break;

				case ZuneController.RightLandscape:
					horizontal = -currGamePadState.ThumbSticks.Left.Y;
					break;
			}

			return horizontal;
		}

		public override Single Vertical()
		{
			Single vertical = 0.0f;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					vertical = currGamePadState.ThumbSticks.Left.X;
					break;

				case ZuneController.RightLandscape:
					vertical = -currGamePadState.ThumbSticks.Left.X;
					break;
			}

			return vertical;
		}

		public override Boolean MenuLeft()
		{
			Boolean menuLeft = false;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					menuLeft = currGamePadState.ThumbSticks.Left.Y < -delta && prevGamePadState.ThumbSticks.Left.Y == 0;
					break;

				case ZuneController.RightLandscape:
					menuLeft = currGamePadState.ThumbSticks.Left.Y > delta && prevGamePadState.ThumbSticks.Left.Y == 0;
					break;
			}

			return menuLeft;
		}

		public override bool MenuRight()
		{
			Boolean menuRight = false;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					menuRight = currGamePadState.ThumbSticks.Left.Y > delta && prevGamePadState.ThumbSticks.Left.Y == 0;
					break;

				case ZuneController.RightLandscape:
					menuRight = currGamePadState.ThumbSticks.Left.Y < -delta && prevGamePadState.ThumbSticks.Left.Y == 0;
					break;
			}

			return menuRight;
		}

		public override bool MenuUp()
		{
			Boolean menuUp = false;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					menuUp = currGamePadState.ThumbSticks.Left.X < -delta && prevGamePadState.ThumbSticks.Left.X == 0;
					break;

				case ZuneController.RightLandscape:
					menuUp = currGamePadState.ThumbSticks.Left.X > delta && prevGamePadState.ThumbSticks.Left.X == 0;
					break;
			}

			return menuUp;
		}

		public override bool MenuDown()
		{
			Boolean menuDown = false;
			switch (ZuneController)
			{
				case ZuneController.LeftLandscape:
					menuDown = currGamePadState.ThumbSticks.Left.X > delta && prevGamePadState.ThumbSticks.Left.X == 0;
					break;

				case ZuneController.RightLandscape:
					menuDown = currGamePadState.ThumbSticks.Left.X < -delta && prevGamePadState.ThumbSticks.Left.X == 0;
					break;
			}

			return menuDown;
		}

		public override void SetMotors(Single leftMotor, Single rightMotor)
		{
		}

		public override void ResetMotor()
		{
		}

	}
}