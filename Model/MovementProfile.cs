using System;
using System.Linq;

namespace PanzerBlitz
{
	public class MovementProfile
	{
		public readonly Tile Tile;

		float[] _RoadMovement = new float[6];
		float[] _NonRoadMovement = new float[6];
		float[] _TruckNonRoadMovement = new float[6];
		float[] _VehicleNonRoadMovement = new float[6];

		public MovementProfile(Tile Tile)
		{
			this.Tile = Tile;
			Recalculate();
		}

		public void Recalculate()
		{
			if (Tile.TileBase == null) return;

			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				Tile n = Tile.NeighborTiles[i];
				if (n == null) continue;

				Edge e = Tile.GetEdge(i);

				_RoadMovement[i] = CalculateMovement(Tile, n, e, true, false, false);
				_NonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, false);
				_TruckNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, true, true);
				_VehicleNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, true);
			}
		}

		public float GetMoveCost(Unit Unit, Tile To, bool RoadMovement)
		{
			int index = Array.IndexOf(Tile.NeighborTiles, To);

			if (RoadMovement
				&& (Tile.Units.Count() == 0 || (Tile.Units.Count() == 1 && Tile.Units.Contains(Unit)))
				&& Tile.PathOverlays.Any(i => i != null && i.RoadMove)
				&& To.PathOverlays.Any(i => i != null && i.RoadMove))
				return _RoadMovement[index];
			else if (Unit.UnitConfiguration.TruckMovement) return _TruckNonRoadMovement[index];
			else if (Unit.UnitConfiguration.IsVehicle) return _VehicleNonRoadMovement[index];
			else return _NonRoadMovement[index];
		}

		private float CalculateMovement(
			Tile From, Tile To, Edge Edge, bool RoadMovement, bool TruckMovement, bool VehicleMovement)
		{
			if (Edge != null && (Edge.NoCrossing || To.TileBase.NoCrossing
				|| (VehicleMovement
					&& (Edge.NoVehicleCrossing || To.TileBase.NoVehicleCrossing)))) return float.MaxValue;

			return CalculateEntryCost(
				To, RoadMovement && From.PathOverlays.Any(i => i != null && i.RoadMove), TruckMovement);
		}

		private float CalculateEntryCost(Tile Tile, bool RoadMovement, bool TruckMovement)
		{
			float move = TruckMovement ? Tile.TileBase.TruckMoveCost : Tile.TileBase.MoveCost;

			float edgeMove = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				Edge e = Tile.GetEdge(i);
				if (e == null) continue;

				float eMove = TruckMovement ? e.TruckMoveCost : e.MoveCost;
				if (eMove > 0 && (!e.RoadMove || RoadMovement)) edgeMove = Math.Min(edgeMove, eMove);
			}

			float pathMove = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				TilePathOverlay p = Tile.GetPathOverlay(i);
				if (p == null) continue;

				float pMove = TruckMovement ? p.TruckMoveCost : p.MoveCost;
				if (pMove > 0 && (!p.RoadMove || RoadMovement)) pathMove = Math.Min(pathMove, pMove);
			}

			if (edgeMove < float.MaxValue) move = edgeMove;
			if (pathMove < float.MaxValue) move = Math.Min(pathMove, move);
			return move;
		}
	}
}
