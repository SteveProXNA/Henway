using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Henway.Graphics
{
	public class Label
	{
		private readonly HenwayGame game;

		public Label(
			HenwayGame game,
			SpriteFont spriteFont,
			Color primaryColor,
			Color secondaryColor,
			LabelEffect labelEffect
			)
		{
			this.game = game;
			SpriteFont = spriteFont;

			PrimaryColor = primaryColor;
			SecondaryColor = secondaryColor;
			LabelEffect = labelEffect;
		}

		public Vector2 GetPosition(String text)
		{
			Text = text;
			Vector2 measureString = SpriteFont.MeasureString(text);

			return measureString;
		}

		public void SetPosition(Single offsetX, Single offsetY)
		{
			// Set secondary position for shadow labels.
			if (LabelEffect == LabelEffect.Shadow)
			{
				Single space = game.DeviceFactory.Scale * 4;

				Vector2 newSecondaryPosition = new Vector2();
				newSecondaryPosition.X = offsetX;
				newSecondaryPosition.Y = offsetY + space;

				SecondaryPosition = newSecondaryPosition;
			}

			// Set primary position unconditionally.
			Vector2 newPrimaryPosition = new Vector2();
			newPrimaryPosition.X = offsetX;
			newPrimaryPosition.Y = offsetY;

			PrimaryPosition = newPrimaryPosition;
		}

		public void Draw()
		{
			if (LabelEffect == LabelEffect.Outline)
			{
				Single space = game.DeviceFactory.Scale * 2;
				game.SpriteBatch.DrawString(SpriteFont, Text, new Vector2(PrimaryPosition.X - space, PrimaryPosition.Y), Color.Black);
				game.SpriteBatch.DrawString(SpriteFont, Text, new Vector2(PrimaryPosition.X + space, PrimaryPosition.Y), Color.Black);
				game.SpriteBatch.DrawString(SpriteFont, Text, new Vector2(PrimaryPosition.X, PrimaryPosition.Y - space), Color.Black);
				game.SpriteBatch.DrawString(SpriteFont, Text, new Vector2(PrimaryPosition.X, PrimaryPosition.Y + space), Color.Black);
			}

			if (LabelEffect == LabelEffect.Shadow)
			{
				game.SpriteBatch.DrawString(SpriteFont, Text, SecondaryPosition, SecondaryColor);
			}

			game.SpriteBatch.DrawString(SpriteFont, Text, PrimaryPosition, PrimaryColor);
		}

		public SpriteFont SpriteFont { get; set; }
		public String Text { get; set; }

		public Vector2 PrimaryPosition { get; set; }
		public Vector2 SecondaryPosition { get; set; }
		public Color PrimaryColor { get; set; }
		public Color SecondaryColor { get; set; }

		public LabelEffect LabelEffect { get; set; }
	}
}