﻿using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Graphing;

namespace PanzerBlitz
{
	public class Unit
	{
		public EventHandler<EventArgs> OnMove;
		public EventHandler<EventArgs> OnRemove;
		public EventHandler<EventArgs> OnDestroy;

		public readonly Army Army;
		public readonly UnitConfiguration UnitConfiguration;

		private IEnumerable<LineOfSight> _FieldOfSight;

		private bool _Deployed;

		private bool _Fired;
		private bool _Moved;
		private bool _Dispersed;

		private Tile _Position;

		public bool Deployed
		{
			get
			{
				return _Deployed;
			}
		}
		public bool Fired
		{
			get
			{
				return _Fired;
			}
		}

		public bool Moved
		{
			get
			{
				return _Moved;
			}
		}

		public bool Dispersed
		{
			get
			{
				return _Dispersed;
			}
		}

		public Tile Position
		{
			get
			{
				return _Position;
			}
		}

		public Unit(Army Army, UnitConfiguration UnitConfiguration)
		{
			this.Army = Army;
			this.UnitConfiguration = UnitConfiguration;
		}

		public void Remove()
		{
			_Position.Exit(this);
			_Deployed = false;
			if (OnRemove != null) OnRemove(this, EventArgs.Empty);
		}

		public void Place(Tile Tile)
		{
			if (_Position != null) _Position.Exit(this);
			_Position = Tile;
			_Position.Enter(this);
			_Deployed = true;
			if (OnMove != null) OnMove(this, new MovementEventArgs());
		}

		public IEnumerable<LineOfSight> GetFieldOfSight()
		{
			if (_FieldOfSight == null)
			{
				Field<Tile> f = new Field<Tile>(_Position, UnitConfiguration.Range, (i, j) => 1);
				_FieldOfSight = f.GetReachableNodes()
						.Select(i => _Position.CalculateLineOfSight(i.Item1))
						.Where(i => i.Final != _Position && i.Verify() == NoLineOfSightReason.NONE);
			}
			return _FieldOfSight;
		}

		public void Fire()
		{
			_Fired = true;
		}

		public void Reset()
		{
			_Fired = false;
			_Moved = false;
			_Dispersed = false;
		}
	}
}
