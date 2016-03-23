using Microsoft.Xna.Framework.Input;

namespace Henway.Input
{
	public class XboxInputState : AbstractInputState
	{
		public XboxInputState()
		{
			ZuneController = ZuneController.None;
			KeysDetection = false;

			// Buttons.
			ButtonsExit = new[] { Buttons.Back, Buttons.B };
			ButtonsStart = Buttons.Start;
			ButtonsPlay = Buttons.A;

			ButtonsQuit = Buttons.Start;
			ButtonsNext = Buttons.A;
			ButtonsBack = Buttons.B;

			ButtonsBoost = Buttons.A;
			ButtonsUnlock = Buttons.X;
		}

	}
}