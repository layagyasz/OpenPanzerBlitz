using System;
using System.Collections.Generic;
using System.Linq;
using Cardamom.Serialization;

namespace PanzerBlitz
{
	public class TileConfiguration : Serializable
	{
		public EventHandler<EventArgs> OnReconfigure;

		TileEdge[] _Edges = new TileEdge[6];
		TilePathOverlay[] _PathOverlays = new TilePathOverlay[6];

		public byte Elevation { get; private set; }
		public bool ElevationTransition { get; private set; }
		public TileBase TileBase { get; private set; } = TileBase.CLEAR;
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

		public TileConfiguration() { }

		public TileConfiguration(TileConfiguration Copy, bool Invert = false)
		{
			Elevation = Copy.Elevation;
			TileBase = Copy.TileBase;

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
			Elevation = Stream.ReadByte();
			ElevationTransition = Stream.ReadBoolean();
			TileBase = (TileBase)Stream.ReadByte();
			_Edges = Stream.ReadArray(i => (TileEdge)Stream.ReadByte());
			_PathOverlays = Stream.ReadArray(i => (TilePathOverlay)Stream.ReadByte());
		}

		public void Serialize(SerializationOutputStream Stream)
		{
			Stream.Write(Elevation);
			Stream.Write(ElevationTransition);
			Stream.Write((byte)TileBase);
			Stream.Write(_Edges, i => Stream.Write((byte)i));
			Stream.Write(_PathOverlays, i => Stream.Write((byte)i));
		}

		public void Merge(TileConfiguration Configuration)
		{
			if (TileBase == TileBase.CLEAR) TileBase = Configuration.TileBase;
			Elevation = Math.Max(Elevation, Configuration.Elevation);
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

		public void SetElevation(byte Elevation)
		{
			this.Elevation = Elevation;
			TriggerReconfigure();
		}

		public void SetElevationTransition(bool ElevationTransition)
		{
			this.ElevationTransition = ElevationTransition;
			TriggerReconfigure();
		}

		public void SetTileBase(TileBase TileBase)
		{
			this.TileBase = TileBase;
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

		public bool HasPathOverlay(TilePathOverlay PathOverlay)
		{
			return _PathOverlays.Any(i => i == PathOverlay);
		}

		public void TriggerReconfigure()
		{
			if (OnReconfigure != null) OnReconfigure(this, EventArgs.Empty);
		}
	}
}
