using Microsoft.Xna.Framework.Input;

namespace Henway.Input
{
	public class WorkInputState : AbstractInputState
	{
		public WorkInputState()
		{
			ZuneController = ZuneController.None;
			KeysDetection = true;

			// Buttons.
			ButtonsExit = new[] { Buttons.Back, Buttons.B };
			ButtonsStart = Buttons.Start;
			ButtonsPlay = Buttons.A;

			ButtonsQuit = Buttons.Start;
			ButtonsNext = Buttons.A;
			ButtonsBack = Buttons.B;

			ButtonsBoost = Buttons.A;
			ButtonsUnlock = Buttons.X;


			// Keys.
			KeysStart = Keys.Enter;
			KeysExit = Keys.Escape;
			KeysPlay = Keys.Enter;

			KeysQuit = Keys.Escape;
			KeysNext = Keys.Enter;
			KeysBack = Keys.Escape;

			KeysBoost = Keys.Space;
			KeysUnlock = Keys.X;

			KeysLeft = Keys.Left;
			KeysRight = Keys.Right;
			KeysUp = Keys.Up;
			KeysDown = Keys.Down;
		}

	}
}