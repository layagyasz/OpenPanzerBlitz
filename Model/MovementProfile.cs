using System;
using System.Linq;

namespace PanzerBlitz
{
	public class MovementProfile
	{
		public readonly Tile Tile;

		float[] RoadMovement = new float[6];
		float[] NonRoadMovement = new float[6];
		float[] TruckNonRoadMovement = new float[6];
		float[] VehicleNonRoadMovement = new float[6];

		public MovementProfile(Tile Tile)
		{
			this.Tile = Tile;
			Recalculate();
		}

		private void Recalculate()
		{
			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				Tile n = Tile.NeighborTiles[i];
				Edge e = Tile.GetEdge(i);

				RoadMovement[i] = CalculateMovement(Tile, n, e, true, false, false);
				NonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, false);
				TruckNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, true, true);
				VehicleNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, true);
			}
		}

		private float CalculateMovement(
			Tile From, Tile To, Edge Edge, bool RoadMovement, bool TruckMovement, bool VehicleMovement)
		{
			if (Edge.NoCrossing || To.TileBase.NoCrossing
				|| (VehicleMovement
					&& (Edge.NoVehicleCrossing || To.TileBase.NoVehicleCrossing))) return float.MaxValue;

			return CalculateEntryCost(To, RoadMovement && From.PathOverlays.Any(i => i.RoadMove), TruckMovement);
		}

		private float CalculateEntryCost(Tile Tile, bool RoadMovement, bool TruckMovement)
		{
			float move = TruckMovement ? Tile.TileBase.TruckMoveCost : Tile.TileBase.MoveCost;

			float edgeMove = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				Edge e = Tile.GetEdge(i);
				float eMove = TruckMovement ? e.TruckMoveCost : e.MoveCost;
				if (eMove > 0 && (!e.RoadMove || RoadMovement)) eMove = Math.Min(edgeMove, eMove);
			}

			float pathMove = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				TilePathOverlay p = Tile.GetPathOverlay(i);
				float pMove = TruckMovement ? p.TruckMoveCost : p.MoveCost;
				if (pMove > 0 && (!p.RoadMove || RoadMovement)) pMove = Math.Min(pathMove, pMove);
			}

			if (edgeMove < float.MaxValue) move = edgeMove;
			if (pathMove < float.MaxValue) move = Math.Min(edgeMove, move);
			return move;
		}
	}
}
