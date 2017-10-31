using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileConfiguration : Serializable
	{
		public EventHandler<EventArgs> OnReconfigure;

		int _Elevation;
		TileBase _TileBase;
		TileEdge[] _Edges = new TileEdge[6];
		TilePathOverlay[] _PathOverlays = new TilePathOverlay[6];

		public int Elevation
		{
			get
			{
				return _Elevation;
			}
		}
		public TileBase TileBase
		{
			get
			{
				return _TileBase;
			}
		}
		public IEnumerable<TilePathOverlay> PathOverlays
		{
			get
			{
				return _PathOverlays;

			}
		}
		public IEnumerable<TileEdge> Edges
		{
			get
			{
				return _Edges;
			}
		}

		public TileConfiguration()
		{
			_TileBase = TileBase.CLEAR;
		}

		public TileConfiguration(TileConfiguration Copy, bool Invert = false)
		{
			_Elevation = Copy._Elevation;
			_TileBase = Copy._TileBase;

			if (Invert)
			{
				for (int i = 0; i < 6; ++i)
				{
					_Edges[i] = Copy._Edges[(i + 3) % 6];
					_PathOverlays[i] = Copy._PathOverlays[(i + 3) % 6];
				}
			}
			else
			{
				_Edges = Copy._Edges.ToArray();
				_PathOverlays = Copy._PathOverlays.ToArray();
			}
		}

		public TileConfiguration(SerializationInputStream Stream)
		{
			_Elevation = Stream.ReadByte();
			_TileBase = (TileBase)Stream.ReadByte();
			_Edges = Stream.ReadArray(i => (TileEdge)Stream.ReadByte());
			_PathOverlays = Stream.ReadArray(i => (TilePathOverlay)Stream.ReadByte());
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write((byte)_Elevation);
			Stream.Write((byte)_TileBase);
			Stream.Write(_Edges, i => Stream.Write((byte)i));
			Stream.Write(_PathOverlays, i => Stream.Write((byte)i));
		}

		public void Merge(TileConfiguration Configuration)
		{
			if (_TileBase == TileBase.CLEAR) _TileBase = Configuration._TileBase;
			_Elevation = Math.Max(_Elevation, Configuration._Elevation);
			for (int i = 0; i < 6; ++i)
			{
				if (_PathOverlays[i] == TilePathOverlay.NONE) _PathOverlays[i] = Configuration._PathOverlays[i];
				if (_Edges[i] == TileEdge.NONE) _Edges[i] = Configuration._Edges[i];
			}
			TriggerReconfigure();
		}

		public void SetPathOverlay(int Index, TilePathOverlay PathOverlay)
		{
			_PathOverlays[Index] = PathOverlay;
			TriggerReconfigure();
		}

		public void SetElevation(int Elevation)
		{
			_Elevation = Elevation;
			TriggerReconfigure();
		}

		public void SetTileBase(TileBase TileBase)
		{
			_TileBase = TileBase;
			TriggerReconfigure();
		}

		public TilePathOverlay GetPathOverlay(int Index)
		{
			return _PathOverlays[Index];
		}

		public TileEdge GetEdge(int Index)
		{
			return _Edges[Index];
		}

		public void SetEdge(int Index, TileEdge Edge)
		{
			_Edges[Index] = Edge;
			TriggerReconfigure();
		}

		public bool HasEdge(TileEdge Edge)
		{
			return _Edges.Any(i => i == Edge);
		}

		public void TriggerReconfigure()
		{
			if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
		}
	}
}
