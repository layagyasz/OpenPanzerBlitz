using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface.Items;
using Cardamom.Utilities;

using SFML.Window;

namespace PanzerBlitz
{
	public class ConvoyDeploymentPage : DeploymentPage
	{
		public EventHandler<EventArgs> OnLoadAction;
		public EventHandler<EventArgs> OnUnloadAction;

		ConvoyDeployment _Deployment;
		readonly UnitConfigurationRenderer _Renderer;
		readonly ValuedScrollCollection<SingularUnitSelectionOption, StackView> _Selection;

		Button _MoveUpButton = new Button("small-button") { DisplayedString = "Up" };
		Button _MoveDownButton = new Button("small-button") { DisplayedString = "Down" };
		Button _LoadButton = new Button("small-button") { DisplayedString = "Load" };
		Button _UnloadButton = new Button("small-button") { DisplayedString = "Unload" };

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;
			}
		}

		public Unit SelectedUnit
		{
			get
			{
				if (_Selection.Value != null)
					return _Selection.Value.Value.Units.FirstOrDefault(i => i.Carrier == null);
				return null;
			}
		}

		public ConvoyDeploymentPage(
			ConvoyDeployment Deployment, UnitConfigurationRenderer Renderer, DeploymentPane Pane)
		{
			_Deployment = Deployment;
			_Renderer = Renderer;

			_MoveUpButton.OnClick += HandleMoveUp;
			_MoveDownButton.OnClick += HandleMoveDown;
			_LoadButton.OnClick += HandleLoadButton;
			_UnloadButton.OnClick += HandleUnloadButton;

			_Selection = new ValuedScrollCollection<SingularUnitSelectionOption, StackView>("deployment-select");

			var units = Deployment.Units.ToList();
			units.Sort(new FluentComparator<Unit>(i => i.Configuration.IsVehicle)
					   .ThenCompare(i => i.Configuration.IsArmored)
					   .ThenCompare(i => i.Configuration.UnitClass != UnitClass.TRANSPORT)
					   .ThenCompare(i => i.Configuration.Movement)
					   .Invert());
			foreach (Unit u in units)
			{
				u.OnLoad += HandleLoad;
				u.OnUnload += HandleUnload;

				var option = new SingularUnitSelectionOption("deployment-selection-option", u, _Renderer);
				_Selection.Add(option);
			}

			_Selection.Position = new Vector2f(0, 48);
			_LoadButton.Position = new Vector2f(0, Pane.Size.Y - _LoadButton.Size.Y - 32);
			_UnloadButton.Position = new Vector2f(_LoadButton.Size.X + 8, Pane.Size.Y - _UnloadButton.Size.Y - 32);
			_MoveUpButton.Position = new Vector2f(0, Pane.Size.Y - _LoadButton.Size.Y - _MoveUpButton.Size.Y - 36);
			_MoveDownButton.Position = new Vector2f(
				_MoveUpButton.Size.X + 8, Pane.Size.Y - _UnloadButton.Size.Y - _MoveDownButton.Size.Y - 36);

			Add(_Selection);
			Add(_LoadButton);
			Add(_UnloadButton);
			Add(_MoveUpButton);
			Add(_MoveDownButton);
		}

		int SortUnits(Unit a, Unit b)
		{
			if (b.Configuration.IsVehicle.CompareTo(a.Configuration.IsVehicle) != 0)
				return b.Configuration.IsVehicle.CompareTo(a.Configuration.IsVehicle);
			if (b.Configuration.IsArmored.CompareTo(a.Configuration.IsArmored) != 0)
				return b.Configuration.IsArmored.CompareTo(a.Configuration.IsArmored);
			bool aTransport = a.Configuration.UnitClass == UnitClass.TRANSPORT;
			bool bTransport = b.Configuration.UnitClass == UnitClass.TRANSPORT;
			if (aTransport.CompareTo(bTransport) != 0)
				return aTransport.CompareTo(bTransport);
			return b.Configuration.Movement.CompareTo(a.Configuration.Movement);
		}

		public IEnumerable<Unit> GetConvoyOrder()
		{
			return _Selection.Select(i => i.Value.Units.FirstOrDefault(j => j.Carrier == null));
		}

		void HandleMoveUp(object Sender, EventArgs E)
		{
			if (_Selection.Value != null)
				_Selection.Move(Math.Max(0, _Selection.ToList().IndexOf(_Selection.Value) - 1), _Selection.Value);
		}

		void HandleMoveDown(object Sender, EventArgs E)
		{
			if (_Selection.Value != null)
				_Selection.Move(
					Math.Min(_Selection.Count() - 1, _Selection.ToList().IndexOf(_Selection.Value) + 1),
					_Selection.Value);
		}

		void HandleLoadButton(object Sender, EventArgs E)
		{
			if (OnLoadAction != null) OnLoadAction(Sender, E);
		}

		void HandleUnloadButton(object Sender, EventArgs E)
		{
			if (OnUnloadAction != null) OnUnloadAction(Sender, E);
		}

		void HandleLoad(object Sender, EventArgs E)
		{
			var u = (Unit)Sender;
			var carrierOption = _Selection.FirstOrDefault(i => i.Unit == u);
			var passengerOption = _Selection.FirstOrDefault(i => i.Unit == u.Passenger);
			_Selection.Remove(passengerOption);
			carrierOption.Value.Merge(passengerOption.Value);
		}

		void HandleUnload(object Sender, ValuedEventArgs<Unit> E)
		{
			var u = (Unit)Sender;
			var carrierOption = _Selection.FirstOrDefault(i => i.Unit == u);
			var option = new SingularUnitSelectionOption("deployment-selection-option", E.Value, _Renderer);
			_Selection.Insert(Math.Min(_Selection.Count() - 1, _Selection.ToList().IndexOf(carrierOption) + 1), option);
			carrierOption.Value.Remove(E.Value);
		}
	}
}
