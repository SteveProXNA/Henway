using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Sprite.Action;

namespace Henway.Sprite
{
	public abstract class AbstractSprite
	{
		public Texture2D Texture { get; set; }
		public Color[] TextureData { get; set; }

		public Int32 Width { get; private set; }
		public Int32 TotalWidth { get; private set; }
		public Int32 Height { get; private set; }
		public Int32 CurFrame { get; set; }
		public Int32 NumFrame { get; set; }
		protected Single Scale { get; set; }

		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }

		public Rectangle Collision { get; private set; }
		public Rectangle Bounds { get; set; }
		public BoundsAction BoundsAction { get; set; }

		protected AbstractSprite(Texture2D texture, Single scale, Int32 numFrame)
		{
			UpdateTexture(texture);
			TotalWidth = texture.Width;
			Scale = scale;

			CurFrame = 0;
			NumFrame = numFrame;
			Width = TotalWidth / NumFrame;
		}

		public virtual void Update()
		{
			// Update the position.
			Vector2 newPosition = new Vector2();
			newPosition.X = Position.X + Velocity.X;
			newPosition.Y = Position.Y + Velocity.Y;

			Vector2 newVelocity = new Vector2();
			newVelocity.X = Velocity.X;
			newVelocity.Y = Velocity.Y;

			// Wrap bounds action.
			if (BoundsAction == BoundsAction.Wrap)
			{
				// Too far left.
				//if (newPosition.X + Width < Bounds.Left)
				//{
				//    newPosition.X = Bounds.Right;
				//}
				// Too far right.
				//else if (newPosition.X > Bounds.Right)
				//{
				//    newPosition.X = Bounds.Left - Width;
				//}
				// Too far up.
				if (newPosition.Y + Height < Bounds.Top)
				{
					newPosition.Y = Bounds.Bottom;
				}
				// Too far down.
				else if (newPosition.Y > Bounds.Bottom)
				{
					newPosition.Y = Bounds.Top - Height;
				}
			}

			// Stop bounds action.
			if (BoundsAction == BoundsAction.Stop)
			{
				// Too far left or right.
				if (newPosition.X < Bounds.Left || newPosition.X > (Bounds.Right - Width))
				{
					newPosition.X = MathHelper.Max(Bounds.Left, MathHelper.Min(newPosition.X, Bounds.Right - Width));
					newVelocity.X = 0;
					newVelocity.Y = 0;
				}
				// Too far up or down.
				if (newPosition.Y < Bounds.Top || newPosition.Y > (Bounds.Bottom - Height))
				{
					newPosition.Y = MathHelper.Max(Bounds.Top, MathHelper.Min(newPosition.Y, Bounds.Bottom - Height));
					newVelocity.X = 0;
					newVelocity.Y = 0;
				}
			}

			// Finish bounds action.
			if (BoundsAction == BoundsAction.Finish)
			{
				// Too far up.
				if (newPosition.Y + Height < Bounds.Top)
				{
					newVelocity.Y = 0;
				}
				// Too far down.
				else if (newPosition.Y > Bounds.Bottom)
				{
					newVelocity.Y = 0;
				}
			}

			Position = newPosition;
			Velocity = newVelocity;

			// Update the collision rectangle.
			Rectangle newCollision = Rectangle;
			Int32 amount = (Int32)(6 * Scale);
			newCollision.Inflate(-amount, -amount);
			Collision = newCollision;
		}

		public virtual void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, DrawRectangle, Color.White);
		}

		public void UpdateTexture(Texture2D texture)
		{
			Texture = texture;
			Height = texture.Height;
		}

		public void UpdateCollision()
		{
			Collision = Rectangle;
		}

		public virtual Rectangle Rectangle
		{
			get
			{
				Rectangle rectangle = new Rectangle(
					(Int32)Position.X,
					(Int32)Position.Y,
					Width,
					Height
					);

				return rectangle;
			}
		}

		public Rectangle DrawRectangle
		{
			get
			{
				Rectangle drawRectangle = new Rectangle(
					CurFrame * Width,
					0,
					Width,
					Height
					);

				return drawRectangle;
			}
		}

	}
}