using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Sprite.Action;

namespace Henway.Sprite
{
	public class CarSprite : AbstractSprite
	{
		public CarSprite(
			Texture2D texture,
			Single scale,
			Vector2 position,
			Vector2 velocity,
			Rectangle bounds,
			BoundsAction boundsAction,
			Single gameWindowHeight,
			Int32 numFrame
			)
			: base(texture, scale, numFrame)
		{
			TextureData = new Color[Width * Height];
			Texture.GetData(TextureData);

			// Correct the starting point.
			position.Y = -texture.Height;
			if (velocity.Y < 0)
			{
				position.Y = gameWindowHeight;
			}

			Position = position;
			Velocity = velocity;
			CarVelocity = velocity;

			CarAction = CarAction.None;
			CarSwerve = CarSwerve.None;

			Bounds = bounds;
			BoundsAction = boundsAction;
		}

		public Int32 Movement { get; set; }
		public Single EndPositionX { get; set; }
		public CarDirection CarDirection { get; set; }
		public CarAction CarAction { get; set; }
		public CarSwerve CarSwerve { get; set; }
		public Vector2 CarVelocity { get; set; }
		public Vector2 EndVelocity { get; set; }
		public Single Delta { get; set; }
		public Single Timer { get; set; }
	}
}