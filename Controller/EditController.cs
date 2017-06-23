using System;

using Cardamom.Interface;

namespace PanzerBlitz
{
	public class EditController
	{
		GameScreen _GameScreen;
		EditPane _EditPane = new EditPane();

		public EditController(GameScreen GameScreen)
		{
			_GameScreen = GameScreen;
			_GameScreen.AddPane(_EditPane);

			foreach (TileView t in _GameScreen.MapView.TilesEnumerable)
			{
				t.OnClick += OnTileClick;
			}
		}

		private void OnTileClick(object sender, MouseEventArgs e)
		{
			_EditPane.EditTile(((TileView)sender).Tile, e.Position);
		}
	}
}
