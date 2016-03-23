using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Henway.State.Abstract;

namespace Henway.State.Concrete
{
	public class GameState01Drive : AbstractGameState
	{
		private StorageState storageState;
		private enum StorageState
		{
			SignIn,
			DeviceSelect,
			EmptySignIn,
			EmptyDevice,
			Action,
			Complete
		};

		public StorageAction StorageActionX { get; set; }
		public EnumGameState SaveEnumGameState { get; set; }

		private String actionText;
		private String emptyText;
		private String currText;
		private String prevText;

		private String[] displayText;

		private Boolean promptSignIn;
		private Boolean promptDevice;
		private Single timer;
		private const Int32 max = 5;
		private Int32 pos;
		private SaveSynchStorage saveSynchStorage;
		private Single synchTimer;
		private const Int32 sleepTime = 800;

		public GameState01Drive(HenwayGame game)
			: base(game)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			actionText = StorageActionX + "...."; ;
			emptyText = String.Format("Continue without {0}?", StorageActionX.ToString().ToLower());

			currText = String.Empty;
			prevText = String.Empty;

			displayText = new String[max];
			Int32 length = StorageActionX.ToString().Length;
			for (Int32 idx = 0; idx < max; idx++)
			{
				String tempText = actionText.Substring(0, idx + length);
				displayText[idx] = tempText;
			}

			storageState = StorageState.SignIn;
			promptSignIn = true;
			promptDevice = true;
			timer = 0;
			pos = 0;
			saveSynchStorage = SaveSynchStorage.None;
			synchTimer = 0;

			// Initialize game label.
			InitializeHenwayLabel();
		}

		public override void UpdateInput(GameTime gameTime)
		{
			base.UpdateInput(gameTime);

			PlayerIndex playerIndex;
			switch (storageState)
			{
				case StorageState.SignIn:
					Int32 signedInGamers = Gamer.SignedInGamers.Count;
					if (signedInGamers < 1)
					{
						if (promptSignIn)
						{
							game.ShowSignIn();
							promptSignIn = false;
						}
						else
						{
							currText = emptyText;
							storageState = StorageState.EmptySignIn;
						}
					}
					else
					{
						Boolean correctSignedGamer = false;
						for (Int32 index = 0; index < signedInGamers; index++)
						{
							playerIndex = Gamer.SignedInGamers[index].PlayerIndex;
							if (playerIndex == inputState.PlayerIndex)
							{
								correctSignedGamer = true;
								break;
							}
						}

						if (correctSignedGamer)
						{
							promptDevice = true;
							storageState = StorageState.DeviceSelect;
						}
						else
						{
							if (promptSignIn)
							{
								game.ShowSignIn();
								promptSignIn = false;
							}
							else
							{
								currText = emptyText;
								storageState = StorageState.EmptySignIn;
							}
						}
					}
					break;

				case StorageState.DeviceSelect:
					if (promptDevice)
					{
						// PlayerIndex will not be null at this point.
						playerIndex = (PlayerIndex)inputState.PlayerIndex;

						promptDevice = false;
						if (StorageActionX == StorageAction.Loading)
						{
							Guide.BeginShowStorageDeviceSelector(playerIndex, StorageCallback, null);
						}
						if (StorageActionX == StorageAction.Saving)
						{
							if (game.HenwayGameVariable.HenwayStorageDevice != null && game.HenwayGameVariable.HenwayStorageDevice.IsConnected)
							{
								pos = 0;
								currText = displayText[pos];
								if (currText != prevText)
								{
									StorageDeviceLabelText();
								}

								saveSynchStorage = SaveSynchStorage.FirstFrame;
								storageState = StorageState.Action;
							}
							else
							{
								Guide.BeginShowStorageDeviceSelector(playerIndex, StorageCallback, null);
							}
						}
					}
					break;

				case StorageState.EmptySignIn:
				case StorageState.EmptyDevice:
					if (currText != prevText)
					{
						StorageDeviceLabelText();
					}

					Boolean back = inputState.Back();
					if (back)
					{
						promptSignIn = true;
						promptDevice = true;
						storageState = StorageState.SignIn;
					}

					Boolean next = inputState.Next();
					if (next)
					{
						if (StorageActionX == StorageAction.Loading)
						{
							// Default high score.
							gameVariable.MaxLevel = 1;
						}

						storageState = StorageState.Complete;
					}

					break;

				case StorageState.Action:
					timer += gameTime.ElapsedGameTime.Milliseconds;
					synchTimer += gameTime.ElapsedGameTime.Milliseconds;
					if (timer > 400)
					{
						timer = 0;
						pos++;
						if (pos >= max)
						{
							pos = 0;
						}

						gameVariable.StorageDeviceLabel[0].Text = displayText[pos];
					}

					if (StorageActionX == StorageAction.Saving)
					{
						if (saveSynchStorage == SaveSynchStorage.FirstFrame)
						{
							saveSynchStorage = SaveSynchStorage.SecondFrame;
						}
						else if (saveSynchStorage == SaveSynchStorage.SecondFrame)
						{
							SaveHighScore();
							saveSynchStorage = SaveSynchStorage.ThirdFrame;
						}
						else if (saveSynchStorage == SaveSynchStorage.ThirdFrame)
						{
							if (synchTimer > sleepTime)
							{
								storageState = StorageState.Complete;
							}
						}
					}
					break;

				case StorageState.Complete:
					if (StorageActionX == StorageAction.Loading)
					{
						gameVariable.NextGameState = EnumGameState.GameState01Title;
					}
					if (StorageActionX == StorageAction.Saving)
					{
						gameVariable.NextGameState = SaveEnumGameState;
					}
					break;
			}
		}

		public override void DrawHighway()
		{
			if (storageState == StorageState.SignIn || storageState == StorageState.DeviceSelect || storageState == StorageState.Complete)
			{
				return;
			}

			base.DrawHighway();
		}

		public override void DrawHud()
		{
			if (storageState == StorageState.SignIn || storageState == StorageState.DeviceSelect || storageState == StorageState.Complete)
			{
				return;
			}

			// Saving.... do not draw title.
			if (storageState == StorageState.Action && StorageActionX == StorageAction.Saving)
			{
				gameVariable.StorageDeviceLabel[0].Draw();
				return;
			}

			// Draw title.
			gameVariable.HenwayLabel.Draw();
			if (!String.IsNullOrEmpty(gameVariable.StorageDeviceLabel[0].Text))
			{
				gameVariable.StorageDeviceLabel[0].Draw();
				if (storageState != StorageState.Action)
				{
					gameVariable.StorageDeviceLabel[1].Draw();
					gameVariable.StorageDeviceLabel[2].Draw();
				}
			}
		}

		private void StorageCallback(IAsyncResult result)
		{
			game.HenwayGameVariable.HenwayStorageDevice = Guide.EndShowStorageDeviceSelector(result);

			// Set up the text to display.
			pos = 0;
			currText = displayText[pos];
			if (game.HenwayGameVariable.HenwayStorageDevice == null)
			{
				currText = emptyText;
			}

			StorageDeviceLabelText();
			if (game.HenwayGameVariable.HenwayStorageDevice == null)
			{
				storageState = StorageState.EmptyDevice;
			}
			else
			{
				storageState = StorageState.Action;
				if (StorageActionX == StorageAction.Loading)
				{
					gameVariable.LoadHighScore();
				}
				if (StorageActionX == StorageAction.Saving)
				{
					SaveHighScore();
				}

				System.Threading.Thread.Sleep(sleepTime);
				storageState = StorageState.Complete;
			}
		}

		private void StorageDeviceLabelText()
		{
			prevText = currText;

			Vector2 measureString = gameVariable.StorageDeviceLabel[0].GetPosition(currText);
			Single offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			Single offsetY = gameWindowY + (gameWindowHeight - measureString.Y) / 2;
			offsetY += scale * 48;

			gameVariable.StorageDeviceLabel[0].SetPosition(offsetX, offsetY);
			gameVariable.StorageDeviceLabel[0].Text = currText;

			String tempText = game.DeviceFactory.NextText;
			measureString = gameVariable.StorageDeviceLabel[1].GetPosition(tempText);
			offsetX = gameWindowX + (gameWindowWidth - measureString.X) / 2;
			offsetY += scale * 48;
			gameVariable.StorageDeviceLabel[1].SetPosition(offsetX, offsetY);
			gameVariable.StorageDeviceLabel[1].Text = tempText;

			offsetY += scale * 36;
			gameVariable.StorageDeviceLabel[2].SetPosition(offsetX, offsetY);
			gameVariable.StorageDeviceLabel[2].Text = game.DeviceFactory.BackText;
		}

		private void SaveHighScore()
		{
			gameVariable.MaxLevel = gameVariable.CarLevel;
			gameVariable.SaveHighScore();
		}

		private enum SaveSynchStorage
		{
			None,
			FirstFrame,
			SecondFrame,
			ThirdFrame
		}

	}
}