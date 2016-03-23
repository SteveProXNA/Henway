using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Sprite.Action;

namespace Henway.Sprite
{
	public class ChickenSprite : AbstractSprite
	{
		private Int32 oneWidth;
		private Int32 oneHeight;

		public ChickenSprite(
			Texture2D texture,
			Texture2D textureOne,
			Single scale,
			Vector2 position,
			Rectangle bounds,
			BoundsAction boundsAction,
			Int32 numFrame
			)
			: base(texture, scale, numFrame)
		{
			// Load color arrays for each individual frame.
			oneWidth = textureOne.Width;
			oneHeight = textureOne.Height;

			TextureData = new Color[oneWidth * oneHeight];
			textureOne.GetData(TextureData);

			Position = position;
			Velocity = Vector2.Zero;

			Bounds = bounds;
			BoundsAction = boundsAction;
		}

		public override Rectangle Rectangle
		{
			get
			{
				Rectangle rectangle = new Rectangle(
					(Int32)Position.X,
					(Int32)Position.Y,
					oneWidth,
					oneHeight
					);

				return rectangle;
			}
		}

	}
}