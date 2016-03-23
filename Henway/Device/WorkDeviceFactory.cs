using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Henway.Input;
using Henway.State.Abstract;

namespace Henway.Device
{
	public class WorkDeviceFactory : AbstractDeviceFactory
	{
		public WorkDeviceFactory(HenwayGame game)
			: base(game)
		{
			InputState = new WorkInputState();

			PreferredBackBufferWidth = 1024;
			PreferredBackBufferHeight = 576;

			GameWindowX = 0;
			GameWindowY = 0;
			GameWindowWidth = 1024;
			GameWindowHeight = 576;

			ContentFolder = "ContentHI";
			Scale = 1.0f;

			SongVolume = 0.2f;
			SoundEffectVolume = 0.6f;

			EndDelta = 0.0025f;
			EndDelta2 = 0.0025f;
			EndVolume = 0.0f;

			MaxCars = 8;
			CarTextureNames = new[] { "Car1", "Car2", "Car3", "Car4", "Car1", "Car2", "Car3", "Car4" };
			CarLeftStart = new Single[] { 97, 212, 317, 427, 532, 647, 752, 862 };
			ChickenCross = 957.0f;

			RoadMapCheck = new[] { 11, 1, 13, 2, 15, 3, 17, 4, 19, 5, 21, 6, 23, 7, 25, 8, 27 };
			RoadMapWidth = new[] { 0, 52, 159, 167, 256, 272, 378, 382, 471, 487, 594, 602, 691, 707, 813, 817, 906 };

			RoadSignPosn = new[] { 171, 280, 389, 494, 607, 716, 825 };
			RoadSignLocn = new[] { "", "", "4", "47", "471", "471", "4715", "47153", "47153", "471536", "4715362", "4715362", "4715362", "4715362", "4715362" };
			RoadSignSize = new[] { "", "", "0", "01", "011", "011", "0111", "01111", "01111", "011111", "0111111", "0111111", "0011111", "0011111", "0001111" };
			RoadSignHigh = "TMBMTMB";

			NextText = "'Enter'  = Yes";
			BackText = "'Escape' = No";
			ContText = "Press 'Enter' to continue";
			RecordText = "You have broken the record!";
			HintText = new[] { "HINT: Press 'Space' for Boost Power!", "HINT: Rest between lanes to avoid cars!", "HINT: Avoid 'Arrow' road signs!" };
		}

	}
}