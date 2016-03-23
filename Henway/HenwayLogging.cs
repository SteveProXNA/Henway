using System;
using Microsoft.Xna.Framework;
using log4net;
using log4net.Config;

namespace HenwayTest
{
	public class HenwayLogging : GameComponent
	{
#if !XBOX && !ZUNE
		protected static readonly ILog log = LogManager.GetLogger(typeof(HenwayGame));
#endif
		private HenwayGame game;

		public HenwayLogging(HenwayGame game)
			: base(game)
		{
			this.game = game;
		}
#if !XBOX && !ZUNE
		public override void Initialize()
		{
			XmlConfigurator.Configure();
			base.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			String message = String.Format("HenwayLogging Update: Curr Game State: {0}  Next Game State: {1}",
				game.HenwayGameVariable.CurrGameState,
				game.HenwayGameVariable.NextGameState
				);

			log.Info(message);

			base.Update(gameTime);
		}
#endif
	}
}