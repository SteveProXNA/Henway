using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Henway.Sprite
{
	public sealed class RoadSignSprite : AbstractSprite
	{
		public RoadSignSprite(
			Texture2D texture,
			Single scale,
			Vector2 position,
			Rectangle bounds,
			Int32 numFrame
			)
			: base(texture, scale, numFrame)
		{
			TextureData = new Color[Width * Height];
			Texture.GetData(TextureData);

			Position = position;
			Velocity = Vector2.Zero;

			Bounds = bounds;
			UpdateCollision();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, BorderRectangle, Color.Black);
			base.Draw(spriteBatch);
		}

		public Rectangle BorderRectangle
		{
			get
			{
				Int32 space = (Int32)(Scale * 2);
				Rectangle borderRectangle = new Rectangle(
				(Int32)Position.X - space,
				(Int32)Position.Y - space,
				Width + 2 * space,
				Height + 2 * space
				);

				return borderRectangle;
			}
		}

	}
}