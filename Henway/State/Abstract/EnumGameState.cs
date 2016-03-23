namespace Henway.State.Abstract
{
	public enum EnumGameState
	{
		GameState00Intro = 0,
		GameState01Drive,
		GameState01Title,
		GameState02Level,
		GameState03Play,
		GameState04Cross,
		GameState05Finish,
		GameState06Dead,
		GameState07Over
	}

	public enum StorageAction
	{
		Loading,
		Saving
	};
}