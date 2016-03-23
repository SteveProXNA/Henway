using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Henway.Device;
using Henway.Input;

namespace Henway.State.Abstract
{
	public abstract partial class AbstractGameState
	{
		protected readonly HenwayGame game;
		protected SpriteBatch spriteBatch;

		// Main variables.
		protected HenwayGameVariable gameVariable;
		protected AbstractDeviceFactory deviceFactory;
		protected AbstractInputState inputState;

		protected AbstractGameState(HenwayGame game)
		{
			this.game = game;
		}

		public virtual void Initialize()
		{
			// Initialize main variables.
			spriteBatch = game.SpriteBatch;
			gameVariable = game.HenwayGameVariable;

			deviceFactory = game.DeviceFactory;
			inputState = deviceFactory.InputState;


			// Initialize helper variables.
			gameWindowX = game.DeviceFactory.GameWindowX;
			gameWindowY = game.DeviceFactory.GameWindowY;
			gameWindowWidth = game.DeviceFactory.GameWindowWidth;
			gameWindowHeight = game.DeviceFactory.GameWindowHeight;
			
			scale = game.DeviceFactory.Scale;
			gameVariable.MasterQuit = false;

			SignedInGamer.SignedIn += SignedInGamer_SignedIn;
			SignedInGamer.SignedOut += SignedInGamer_SignedOut;
		}

		public virtual void LoadContent()
		{
		}

		// Template method for Update.
		public void Update(GameTime gameTime)
		{
			if (!game.IsActive)
			{
				return;
			}

			UpdateInput(gameTime);

			// If master quit flag true then do not update sprites or general.
			if (gameVariable.MasterQuit)
			{
				return;
			}

			UpdateSprites(gameTime);
			UpdateGeneral(gameTime);
		}

		// Template method for Draw.
		public void Draw(GameTime gameTime)
		{
			if (!game.IsActive)
			{
				return;
			}

			DrawHighway();
			DrawChicken();
			DrawCars();
			DrawHud();
		}

		// Helper variables used in all game states.
		protected Int32 gameWindowX;
		protected Int32 gameWindowY;
		protected Int32 gameWindowWidth;
		protected Int32 gameWindowHeight;
		protected Single scale;

		private void SignedInGamer_SignedIn(object sender, SignedInEventArgs e)
		{
			// Check if correct player is trying to sign in.
			Nullable<PlayerIndex> playerIndex = game.DeviceFactory.InputState.PlayerIndex;
			if (playerIndex != e.Gamer.PlayerIndex)
			{
				return;
			}

			// Set the Intro label.
			gameVariable.SignedInGamer = e.Gamer;
			ResetGamerTag();

			// Set the Trial label.
			Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
			if (isTrialMode)
			{
				if (gameVariable.TrialTitleLabel != null)
				{
					ResetTrialLabel("Press 'X' to unlock game");
				}
			}

			// Set the Level label.
			if (gameVariable.LevelLabel != null)
			{
				gameVariable.LevelLabel[0].Text = gameVariable.SignedInGamer.Gamertag;
			}
		}

		private void SignedInGamer_SignedOut(object sender, SignedOutEventArgs e)
		{
			// Check if correct player is trying to sign out.
			Nullable<PlayerIndex> playerIndex = game.DeviceFactory.InputState.PlayerIndex;
			if (playerIndex != e.Gamer.PlayerIndex)
			{
				return;
			}

			// Set the Intro label.
			gameVariable.SignedInGamer = null;
			ResetGamerTag();

			// Set the Trial label.
			Boolean isTrialMode = game.DeviceFactory.IsTrialMode();
			if (isTrialMode)
			{
				if (gameVariable.TrialTitleLabel != null)
				{
					ResetTrialLabel("Sign in to unlock game");
				}
			}

			// Set the Level label.
			if (gameVariable.LevelLabel != null)
			{
				gameVariable.LevelLabel[0].Text = "GUEST";
			}
		}

		protected void ResetGamerTag()
		{
			String text = "Welcome to...";
			if (gameVariable.SignedInGamer != null)
			{
				text = String.Format("{0}, welcome to...",
					gameVariable.SignedInGamer.Gamertag
					);
			}

			// Set the Intro label.
			if (gameVariable.IntroTitleLabel != null)
			{
				gameVariable.IntroTitleLabel.Text = text;
			}
		}

		private void ResetTrialLabel(String text)
		{
			Vector2 measureString = gameVariable.TrialTitleLabel.GetPosition(text);

			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			gameVariable.TrialTitleLabel.SetPosition(offsetX, gameVariable.TrialTitleLabel.PrimaryPosition.Y);
		}

	}
}