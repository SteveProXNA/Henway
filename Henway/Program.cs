using System;

namespace Henway
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			using (HenwayGame game = new HenwayGame())
			{
				game.Run();
			}
		}
	}
}