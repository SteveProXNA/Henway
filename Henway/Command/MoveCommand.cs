using System;
using Microsoft.Xna.Framework;
using Henway.Sprite;

namespace Henway.Command
{
	public struct MoveCommand : ICommand
	{
		private readonly ChickenSprite chickenSprite;
		public Vector2 Velocity { get; set; }
		public Int32 CurFrame { get; set; }

		public MoveCommand(ChickenSprite chickenSprite, Vector2 velocity, Int32 curFrame)
			: this()
		{
			this.chickenSprite = chickenSprite;
			Velocity = velocity;
			CurFrame = curFrame;
		}

		public void Execute()
		{
			chickenSprite.Velocity = Velocity;
			chickenSprite.CurFrame = CurFrame;
			chickenSprite.Update();
		}

	}
}