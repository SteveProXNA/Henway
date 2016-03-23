using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Henway.Input
{
	public abstract class AbstractInputState
	{
		protected GamePadState currGamePadState { get; set; }
		protected GamePadState prevGamePadState { get; set; }
		protected KeyboardState currKeyboardState { get; set; }
		protected KeyboardState prevKeyboardState { get; set; }
		protected const Single delta = 0.15f;
		private const Single tolerance = 0.3f;

		protected AbstractInputState()
		{
			// Default to empty PlayerIndex.
			PlayerIndex = null;
		}

		public void Update()
		{
			prevGamePadState = currGamePadState;
			if (PlayerIndex != null)
			{
				PlayerIndex tempPlayerIndex = (PlayerIndex)PlayerIndex;
				currGamePadState = GamePad.GetState(tempPlayerIndex, GamePadDeadZone.IndependentAxes);
			}

			if (KeysDetection)
			{
				prevKeyboardState = currKeyboardState;
				currKeyboardState = Keyboard.GetState();
			}
		}

		public virtual Single Horizontal()
		{
			Single horizontal = currGamePadState.ThumbSticks.Left.X;
			if (horizontal == 0)
			{
				if (currGamePadState.IsButtonDown(Buttons.DPadLeft))
				{
					horizontal = -1;
				}
				if (currGamePadState.IsButtonDown(Buttons.DPadRight))
				{
					horizontal = 1;
				}
			}

			if (KeysDetection && horizontal == 0)
			{
				if (currKeyboardState.IsKeyDown(KeysLeft))
				{
					horizontal = -1;
				}
				if (currKeyboardState.IsKeyDown(KeysRight))
				{
					horizontal = 1;
				}
			}

			return horizontal;
		}

		public virtual Single Vertical()
		{
			Single vertical = -currGamePadState.ThumbSticks.Left.Y;
			if (vertical == 0)
			{
				if (currGamePadState.IsButtonDown(Buttons.DPadUp))
				{
					vertical = -1;
				}
				if (currGamePadState.IsButtonDown(Buttons.DPadDown))
				{
					vertical = 1;
				}
			}

			if (KeysDetection && vertical == 0)
			{
				if (currKeyboardState.IsKeyDown(KeysUp))
				{
					vertical = -1;
				}
				if (currKeyboardState.IsKeyDown(KeysDown))
				{
					vertical = 1;
				}
			}

			return vertical;
		}

		public void ZuneControllerSwitch()
		{
			if (currGamePadState.Buttons.Back == ButtonState.Pressed &&
				prevGamePadState.Buttons.Back == ButtonState.Released)
			{
				if (ZuneController == ZuneController.LeftLandscape)
				{
					ZuneController = ZuneController.RightLandscape;
				}
				else if (ZuneController == ZuneController.RightLandscape)
				{
					ZuneController = ZuneController.LeftLandscape;
				}
			}
		}

		public Boolean Exit()
		{
			Boolean exit = false;

			Int32 length = ButtonsExit.Length;
			for (Int32 index = 0; index < length; index++)
			{
				Buttons button = ButtonsExit[index];
				for (PlayerIndex playerIndex = Microsoft.Xna.Framework.PlayerIndex.One; playerIndex <= Microsoft.Xna.Framework.PlayerIndex.Four; playerIndex++)
				{
					exit = GamePad.GetState(playerIndex).IsButtonDown(button);
					if (exit)
					{
						index = length;
						break;
					}
				}
			}

			if (KeysDetection && !exit)
			{
				exit = currKeyboardState.IsKeyDown(KeysExit);
			}

			return exit;
		}

		public Boolean Start()
		{
			Boolean start = false;
			for (PlayerIndex playerIndex = Microsoft.Xna.Framework.PlayerIndex.One; playerIndex <= Microsoft.Xna.Framework.PlayerIndex.Four; playerIndex++)
			{
				start = GamePad.GetState(playerIndex).IsButtonDown(ButtonsStart);
				if (!start)
				{
					continue;
				}

				PlayerIndex = playerIndex;
				break;
			}

			if (KeysDetection && !start)
			{
				start = currKeyboardState.IsKeyDown(KeysStart) && prevKeyboardState.IsKeyUp(KeysStart);
				if (start)
				{
					// Windows only has PlayerIndex One.
					PlayerIndex = Microsoft.Xna.Framework.PlayerIndex.One;
				}
			}

			return start;
		}

		public Boolean Play()
		{
			Boolean play = currGamePadState.IsButtonDown(ButtonsPlay) && prevGamePadState.IsButtonUp(ButtonsPlay);
			if (KeysDetection && !play)
			{
				play = currKeyboardState.IsKeyDown(KeysPlay) && prevKeyboardState.IsKeyUp(KeysPlay);
			}

			return play;
		}

		public Boolean Quit()
		{
			Boolean quit = currGamePadState.IsButtonDown(ButtonsQuit) && prevGamePadState.IsButtonUp(ButtonsQuit);
			if (KeysDetection && !quit)
			{
				quit = currKeyboardState.IsKeyDown(KeysQuit) && prevKeyboardState.IsKeyUp(KeysQuit);
			}

			// Reset input detection as same button may be used for quit and back action.
			if (quit)
			{
				Update();
			}

			return quit;
		}

		public Boolean Next()
		{
			Boolean next = currGamePadState.IsButtonDown(ButtonsNext) && prevGamePadState.IsButtonUp(ButtonsNext);
			if (KeysDetection && !next)
			{
				next = currKeyboardState.IsKeyDown(KeysNext) && prevKeyboardState.IsKeyUp(KeysNext);
			}

			return next;
		}

		public Boolean Back()
		{
			Boolean back = currGamePadState.IsButtonDown(ButtonsBack) && prevGamePadState.IsButtonUp(ButtonsBack);
			if (KeysDetection && !back)
			{
				back = currKeyboardState.IsKeyDown(KeysBack) && prevKeyboardState.IsKeyUp(KeysBack);
			}

			return back;
		}

		public Boolean Boost()
		{
			Boolean boost = currGamePadState.IsButtonDown(ButtonsBoost);
			if (KeysDetection && !boost)
			{
				boost = currKeyboardState.IsKeyDown(KeysBoost);
			}

			return boost;
		}

		public virtual Boolean MenuLeft()
		{
			Boolean menuLeft = currGamePadState.ThumbSticks.Left.X > -tolerance && prevGamePadState.ThumbSticks.Left.X < -tolerance;
			//Boolean menuLeft = currGamePadState.IsButtonDown(Buttons.LeftThumbstickLeft) && prevGamePadState.IsButtonUp(Buttons.LeftThumbstickLeft);
			if (!menuLeft)
			{
				menuLeft = currGamePadState.IsButtonDown(Buttons.DPadLeft) && prevGamePadState.IsButtonUp(Buttons.DPadLeft);
			}
			if (KeysDetection & !menuLeft)
			{
				menuLeft = currKeyboardState.IsKeyDown(KeysLeft) && prevKeyboardState.IsKeyUp(KeysLeft);
			}

			return menuLeft;
		}

		public virtual Boolean MenuRight()
		{
			Boolean menuRight = currGamePadState.ThumbSticks.Left.X > tolerance && prevGamePadState.ThumbSticks.Left.X < tolerance;
			//Boolean menuRight = currGamePadState.IsButtonDown(Buttons.LeftThumbstickRight) && prevGamePadState.IsButtonUp(Buttons.LeftThumbstickRight);
			if (!menuRight)
			{
				menuRight = currGamePadState.IsButtonDown(Buttons.DPadRight) && prevGamePadState.IsButtonUp(Buttons.DPadRight);
			}
			if (KeysDetection & !menuRight)
			{
				menuRight = currKeyboardState.IsKeyDown(KeysRight) && prevKeyboardState.IsKeyUp(KeysRight);
			}

			return menuRight;
		}

		public virtual Boolean MenuUp()
		{
			Boolean menuUp = currGamePadState.ThumbSticks.Left.Y > tolerance && prevGamePadState.ThumbSticks.Left.Y < tolerance;
			//Boolean menuUp = currGamePadState.IsButtonDown(Buttons.LeftThumbstickUp) && prevGamePadState.IsButtonUp(Buttons.LeftThumbstickUp);
			if (!menuUp)
			{
				menuUp = currGamePadState.IsButtonDown(Buttons.DPadUp) && prevGamePadState.IsButtonUp(Buttons.DPadUp);
			}
			if (KeysDetection & !menuUp)
			{
				menuUp = currKeyboardState.IsKeyDown(KeysUp) && prevKeyboardState.IsKeyUp(KeysUp);
			}

			return menuUp;
		}

		public virtual Boolean MenuDown()
		{
			Boolean menuDown = currGamePadState.ThumbSticks.Left.Y > -tolerance && prevGamePadState.ThumbSticks.Left.Y < -tolerance;
			//Boolean menuDown = currGamePadState.IsButtonDown(Buttons.LeftThumbstickDown) && prevGamePadState.IsButtonUp(Buttons.LeftThumbstickDown);
			if (!menuDown)
			{
				menuDown = currGamePadState.IsButtonDown(Buttons.DPadDown) && prevGamePadState.IsButtonUp(Buttons.DPadDown);
			}
			if (KeysDetection & !menuDown)
			{
				menuDown = currKeyboardState.IsKeyDown(KeysDown) && prevKeyboardState.IsKeyUp(KeysDown);
			}

			return menuDown;
		}

		public Boolean Unlock()
		{
			Boolean unlock = currGamePadState.IsButtonDown(ButtonsUnlock) && prevGamePadState.IsButtonUp(ButtonsUnlock);
			if (KeysDetection && !unlock)
			{
				unlock = currKeyboardState.IsKeyDown(KeysUnlock) && prevKeyboardState.IsKeyUp(KeysUnlock);
			}

			return unlock;
		}

		public virtual void SetMotors(Single leftMotor, Single rightMotor)
		{
			if (currGamePadState.IsConnected)
			{
				PlayerIndex playerIndex = (PlayerIndex)PlayerIndex;
				GamePad.SetVibration(playerIndex, leftMotor, rightMotor);
			}
		}

		public virtual void ResetMotor()
		{
			SetMotors(0, 0);
		}

		public Nullable<PlayerIndex> PlayerIndex { get; set; }
		public ZuneController ZuneController { get; set; }
		protected Boolean KeysDetection { get; set; }

		// Buttons.
		protected Buttons[] ButtonsExit { get; set; }
		protected Buttons ButtonsStart { get; set; }
		protected Buttons ButtonsPlay { get; set; }

		protected Buttons ButtonsQuit { get; set; }
		protected Buttons ButtonsNext { get; set; }
		protected Buttons ButtonsBack { get; set; }

		protected Buttons ButtonsBoost { get; set; }
		protected Buttons ButtonsUnlock { get; set; }

		// Keys.
		protected Keys KeysStart { get; set; }
		protected Keys KeysExit { get; set; }
		protected Keys KeysPlay { get; set; }

		protected Keys KeysQuit { get; set; }
		protected Keys KeysNext { get; set; }
		protected Keys KeysBack { get; set; }

		protected Keys KeysBoost { get; set; }
		protected Keys KeysUnlock { get; set; }

		protected Keys KeysLeft { get; set; }
		protected Keys KeysRight { get; set; }
		protected Keys KeysUp { get; set; }
		protected Keys KeysDown { get; set; }
	}

	public enum ZuneController
	{
		None = 0,
		LeftLandscape,
		RightLandscape
	}

}