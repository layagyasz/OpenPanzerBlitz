using System;
using System.Collections.Generic;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class MapRegion : Serializable
	{
		public EventHandler<EventArgs> OnChange;

		string _Name;
		readonly HashSet<Tile> _Tiles;

		public string Name
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
				if (OnChange != null) OnChange(this, EventArgs.Empty);
			}
		}
		public IEnumerable<Tile> Tiles
		{
			get
			{
				return _Tiles;
			}
		}

		public MapRegion()
		{
			_Tiles = new HashSet<Tile>();
		}

		public MapRegion(string Name, IEnumerable<Tile> Tiles)
		{
			_Name = Name;
			_Tiles = new HashSet<Tile>(Tiles);
		}

		public MapRegion(SerializationInputStream Stream, Tile[,] Tiles)
			: this(Stream.ReadString(), Stream.ReadEnumerable(i => Tiles[Stream.ReadInt32(), Stream.ReadInt32()])) { }

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(_Name);
			Stream.Write(_Tiles, i => Stream.Write(i.Coordinate));
		}

		public void Add(Tile Tile)
		{
			if (_Tiles.Add(Tile) && OnChange != null) OnChange(this, EventArgs.Empty);
		}

		public void Remove(Tile Tile)
		{
			if (_Tiles.Remove(Tile) && OnChange != null) OnChange(this, EventArgs.Empty);
		}

		public bool Contains(Tile Tile)
		{
			return _Tiles.Contains(Tile);
		}
	}
}
