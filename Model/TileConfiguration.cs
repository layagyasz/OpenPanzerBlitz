using System;
using System.Linq;

namespace PanzerBlitz
{
	public class TileConfiguration
	{
		public readonly Tile Tile;

		bool _MustAttackAllUnits;
		bool _TreatUnitsAsArmored;
		bool _Depressed;
		bool _DepressedTransition;
		int _DieModifier;
		int _TrueElevation;

		float[] _RoadMovement = new float[6];
		float[] _NonRoadMovement = new float[6];
		float[] _TruckNonRoadMovement = new float[6];
		float[] _VehicleNonRoadMovement = new float[6];

		public bool MustAttackAllUnits
		{
			get
			{
				return _MustAttackAllUnits;
			}
		}
		public bool TreatUnitsAsArmored
		{
			get
			{
				return _TreatUnitsAsArmored;
			}
		}
		public bool Depressed
		{
			get
			{
				return _Depressed;
			}
		}
		public bool DepressedTransition
		{
			get
			{
				return _DepressedTransition;
			}
		}
		public int DieModifier
		{
			get
			{
				return _DieModifier;
			}
		}
		public int TrueElevation
		{
			get
			{
				return _TrueElevation;
			}
		}

		public TileConfiguration(Tile Tile)
		{
			this.Tile = Tile;
			Recalculate();
		}

		public bool CanMove(Unit Unit, Tile To, bool RoadMovement, bool IgnoreOccupyingUnits = false)
		{
			return Math.Abs(float.MaxValue - GetMoveCost(Unit, To, RoadMovement, IgnoreOccupyingUnits)) > float.Epsilon;
		}

		public float GetMoveCost(Unit Unit, Tile To, bool RoadMovement, bool IgnoreOccupyingUnits = false)
		{
			if (!IgnoreOccupyingUnits && Unit.CanEnter(To) == NoDeployReason.ENEMY_OCCUPIED) return float.MaxValue;

			BlockType toBlock = To.GetBlockType();
			BlockType fromBlock = Tile.GetBlockType();

			if (toBlock == BlockType.HARD_BLOCK && (Unit.Moved || !To.NeighborTiles.Any(i => i.Units.Contains(Unit))))
				return float.MaxValue;
			if ((fromBlock == BlockType.HARD_BLOCK || fromBlock == BlockType.SOFT_BLOCK) && Unit.Moved)
				return float.MaxValue;

			int index = Array.IndexOf(Tile.NeighborTiles, To);
			TilePathOverlay pathOverlay = Tile.GetPathOverlay(To);

			if (RoadMovement
				&& pathOverlay != null
				&& pathOverlay.RoadMove
				&& (Tile.Units.Count() == 0 || (Tile == Unit.Position && Unit.IsSolitary())))
				return _RoadMovement[index];
			if (Unit.Configuration.TruckMovement) return _TruckNonRoadMovement[index];
			if (Unit.Configuration.IsVehicle) return _VehicleNonRoadMovement[index];
			return _NonRoadMovement[index];
		}

		public void Recalculate()
		{
			if (Tile.TileBase == null) return;

			_MustAttackAllUnits = Tile.TileBase.MustAttackAllUnits
									  || Tile.Edges.Any(i => i != null && i.MustAttackAllUnits)
									  || Tile.PathOverlays.Any(i => i != null && i.MustAttackAllUnits);
			_TreatUnitsAsArmored = Tile.TileBase.TreatUnitsAsArmored
									   || Tile.Edges.Any(i => i != null && i.TreatUnitsAsArmored)
									   || Tile.PathOverlays.Any(i => i != null && i.TreatUnitsAsArmored);
			_Depressed = Tile.TileBase.Depressed
							 || Tile.Edges.Any(i => i != null && i.Depressed)
							 || Tile.PathOverlays.Any(i => i != null && i.Depressed);
			_DepressedTransition = Tile.TileBase.DepressedTransition
									   || Tile.Edges.Any(i => i != null && i.DepressedTransition)
									   || Tile.PathOverlays.Any(i => i != null && i.DepressedTransition);
			_DieModifier =
				Math.Max(
					Tile.TileBase.DieModifier,
					Math.Max(
						Tile.Edges.Max(i => i == null ? 0 : i.DieModifier),
						Tile.PathOverlays.Max(i => i == null ? 0 : i.DieModifier)));
			_TrueElevation = 2 * Tile.Elevation + (Tile.TileBase.Elevated
												|| Tile.Edges.Any(i => i != null && i.Elevated)
												|| Tile.PathOverlays.Any(i => i != null && i.Elevated) ? 1 : 0);

			for (int i = 0; i < Tile.NeighborTiles.Length; ++i)
			{
				Tile n = Tile.NeighborTiles[i];
				if (n == null) continue;

				Edge e = Tile.GetEdge(i);

				_RoadMovement[i] = CalculateMovement(
					Tile, n, e, true, false, false);
				_NonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, false);
				_TruckNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, true, true);
				_VehicleNonRoadMovement[i] = CalculateMovement(Tile, n, e, false, false, true);
			}
		}

		float CalculateMovement(
			Tile From, Tile To, Edge Edge, bool RoadMovement, bool TruckMovement, bool VehicleMovement)
		{
			bool blockingEdge = Edge != null && (Edge.NoCrossing || (VehicleMovement && Edge.NoVehicleCrossing));

			if (!RoadMovement
				&& (blockingEdge || To.TileBase.NoCrossing || (VehicleMovement && To.TileBase.NoVehicleCrossing)))
				return float.MaxValue;

			float move = TruckMovement ? To.TileBase.TruckMoveCost : To.TileBase.MoveCost;

			float edgeMove = float.MaxValue;
			for (int i = 0; i < 6; ++i)
			{
				Edge e = To.GetEdge(i);
				if (e == null) continue;

				float eMove = TruckMovement ? e.TruckMoveCost : e.MoveCost;
				if (eMove > 0 && (!e.RoadMove || RoadMovement)) edgeMove = Math.Min(edgeMove, eMove);
			}

			float pathMove = float.MaxValue;
			float pathLeave = float.MaxValue;
			bool roaded = false;
			for (int i = 0; i < 6; ++i)
			{
				TilePathOverlay p = To.GetPathOverlay(i);
				if (p != null)
				{
					float pMove = TruckMovement ? p.TruckMoveCost : p.MoveCost;
					if (pMove > 0 && (!p.RoadMove || RoadMovement)) pathMove = Math.Min(pathMove, pMove);
					if (p.RoadMove) roaded = true;
				}

				TilePathOverlay p2 = From.GetPathOverlay(i);
				if (p2 != null)
				{
					float pLeave = TruckMovement ? p2.TruckLeaveCost : p2.LeaveCost;
					if (pLeave > 0 && (!p2.RoadMove || RoadMovement)) pathLeave = Math.Min(pathLeave, pLeave);
				}
			}

			if (edgeMove < float.MaxValue) move = edgeMove;
			if (pathMove < float.MaxValue) move = Math.Min(pathMove, move);
			if (From.Configuration.Depressed
				&& !To.Configuration.Depressed
				&& !To.Configuration.DepressedTransition
				&& !roaded
				&& pathLeave < float.MaxValue)
				move += pathLeave;

			return move;
		}
	}
}