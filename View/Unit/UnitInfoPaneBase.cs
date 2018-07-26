using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public abstract class UnitInfoPaneBase : Pane
	{
		public EventHandler<EventArgs> OnClose;

		UnitConfiguration _UnitConfiguration;
		Faction _Faction;

		Button _CloseButton = new Button("unit-info-close-button") { DisplayedString = "X" };
		UnitConfigurationView _UnitConfigurationView;

		protected ScrollCollection<ClassedGuiItem> _Info =
			new ScrollCollection<ClassedGuiItem>("unit-info-display") { Position = new Vector2f(0, 208) };

		protected UnitInfoPaneBase(
			UnitConfiguration UnitConfiguration, Faction Faction, UnitConfigurationRenderer Renderer)
			: base("unit-info-pane")
		{
			_UnitConfiguration = UnitConfiguration;
			_Faction = Faction;

			_CloseButton.Position = new Vector2f(Size.X - _CloseButton.Size.X - LeftPadding.X * 2, 0);
			_CloseButton.OnClick += HandleClose;

			_UnitConfigurationView = new UnitConfigurationView(UnitConfiguration, Faction, Renderer, 192);
			_UnitConfigurationView.Position = new Vector2f(144, 96);

			Add(_CloseButton);
			Add(_UnitConfigurationView);
			Add(_Info);
		}

		void HandleClose(object Sender, EventArgs E)
		{
			if (OnClose != null) OnClose(this, EventArgs.Empty);
		}

		protected void AddSection(string Section)
		{
			_Info.Add(new Button("unit-info-header-2")
			{
				DisplayedString = Section
			});
		}

		protected void AddAttribute(string Attribute)
		{
			_Info.Add(new Button("unit-info-attribute")
			{
				DisplayedString = Attribute
			});
		}

		protected void AddBasicAttributes()
		{
			AddSection("Basic Attributes");
			AddAttribute(string.Format(
				"Unit Class - {0}", ObjectDescriber.Describe(_UnitConfiguration.UnitClass)));
			AddAttribute(
				string.Format(
					"Weapon Class - {0}", ObjectDescriber.Describe(_UnitConfiguration.PrimaryWeapon.WeaponClass)));
			AddAttribute(string.Format("Attack - {0}", _UnitConfiguration.PrimaryWeapon.Attack));
			AddAttribute(string.Format("Range - {0}", _UnitConfiguration.PrimaryWeapon.Range));
			AddAttribute(string.Format("Defense - {0}", _UnitConfiguration.Defense));
			AddAttribute(
				string.Format(
					"Movement - {0}",
					_UnitConfiguration.HasUnlimitedMovement() ? "Unlimited" : _UnitConfiguration.Movement.ToString()));
			AddAttribute(
				string.Format("Point Value - {0}", _UnitConfiguration.GetPointValue(_Faction.HalfPriceTrucks)));
		}

		protected void AddComposition()
		{
			AddSection("Composition");
			AddAttribute(string.Format("Weight - {0}", _UnitConfiguration.UnitWeight));
			if (_UnitConfiguration.IsVehicle) AddAttribute("Vehicular");
			if (_UnitConfiguration.IsArmored) AddAttribute("Armored");
			if (_UnitConfiguration.WrecksAs != null)
				AddAttribute(string.Format("Wrecks As - {0}", ObjectDescriber.Describe(_UnitConfiguration.WrecksAs)));
		}

		protected void AddCombatCapabilities()
		{
			if (_UnitConfiguration.IsEngineer
				|| _UnitConfiguration.IsParatroop
				|| _UnitConfiguration.IsCommando
				|| _UnitConfiguration.CanDirectFire
				|| _UnitConfiguration.CanIndirectFire
				|| _UnitConfiguration.CanOverrun
				|| _UnitConfiguration.CanOnlyOverrunUnarmored
				|| _UnitConfiguration.CanCloseAssault
				|| _UnitConfiguration.CanOnlySupportCloseAssault
				|| _UnitConfiguration.CanAntiAircraft
				|| _UnitConfiguration.PrimaryWeapon.CanDoubleRange
				|| _UnitConfiguration.CanClearMines
				|| _UnitConfiguration.CanPlaceMines
				|| _UnitConfiguration.CanPlaceBridges
				|| _UnitConfiguration.InnatelyClearsMines
				|| _UnitConfiguration.ImmuneToMines
				|| _UnitConfiguration.HasLowProfile)
				AddSection("Combat Capabilities");
			if (_UnitConfiguration.IsEngineer) AddAttribute("Engineers");
			if (_UnitConfiguration.IsParatroop) AddAttribute("Paratroops");
			if (_UnitConfiguration.IsCommando) AddAttribute("Commandos");
			if (_UnitConfiguration.CanDirectFire) AddAttribute("Direct Fire");
			if (_UnitConfiguration.CanIndirectFire)
			{
				AddAttribute("Indirect Fire");
				AddAttribute(
					string.Format("Minimum Indirect Fire Range - {0}", _UnitConfiguration.MinimumIndirectFireRange));
			}
			if (_UnitConfiguration.CanOverrun) AddAttribute("Overrun");
			if (_UnitConfiguration.CanOnlyOverrunUnarmored) AddAttribute("Overrun Un-armored");
			if (_UnitConfiguration.CanCloseAssault) AddAttribute("Close Assault");
			if (_UnitConfiguration.CanOnlySupportCloseAssault) AddAttribute("Close Assault Support");
			if (_UnitConfiguration.CanAntiAircraft) AddAttribute("Anti-Aircraft");
			if (_UnitConfiguration.PrimaryWeapon.CanDoubleRange) AddAttribute("Can Double Range");
			if (_UnitConfiguration.CanClearMines) AddAttribute("Can Clear Mines");
			if (_UnitConfiguration.CanPlaceMines) AddAttribute("Can Place Mines");
			if (_UnitConfiguration.CanPlaceBridges) AddAttribute("Can Place Bridges");
			if (_UnitConfiguration.InnatelyClearsMines) AddAttribute("Innately Clears Mines");
			if (_UnitConfiguration.ImmuneToMines) AddAttribute("Immune To Mines");
			if (_UnitConfiguration.HasLowProfile) AddAttribute("Low Profile");
		}

		protected void AddSightCapabilities()
		{
			if (_UnitConfiguration.SightRange > 0
				|| _UnitConfiguration.SpotRange > 0
				|| _UnitConfiguration.CanReveal)
				AddSection("Sight Capabilities");
			if (_UnitConfiguration.SightRange > 0)
				AddAttribute(string.Format("Sight Range - {0}", _UnitConfiguration.SightRange));
			if (_UnitConfiguration.SpotRange > 0)
				AddAttribute(string.Format("Spot Range - {0}", _UnitConfiguration.SpotRange));
			if (_UnitConfiguration.CanReveal) AddAttribute("Can Reveal");
		}

		protected void AddTransportCapabilities()
		{
			if (_UnitConfiguration.IsPassenger
				|| _UnitConfiguration.IsOversizedPassenger
				|| _UnitConfiguration.IsCarrier
				|| _UnitConfiguration.UnloadsWhenDisrupted
				|| _UnitConfiguration.CanOnlyCarryLight
				|| _UnitConfiguration.CanOnlyCarryInfantry
				|| _UnitConfiguration.CanCarryInWater)
				AddSection("Transport Capabilities");
			if (_UnitConfiguration.IsPassenger) AddAttribute("Passenger");
			if (_UnitConfiguration.IsOversizedPassenger) AddAttribute("Oversized Passenger");
			if (_UnitConfiguration.IsCarrier) AddAttribute("Carrier");
			if (_UnitConfiguration.UnloadsWhenDisrupted) AddAttribute("Unloads When Disrupted");
			if (_UnitConfiguration.CanOnlyCarryLight) AddAttribute("Carry Only Light Units");
			if (_UnitConfiguration.CanOnlyCarryInfantry) AddAttribute("Carry Only Infantry");
			if (_UnitConfiguration.CanCarryInWater) AddAttribute("Can Carry In Water");
		}

		protected void AddMovementAttributes()
		{
			AddSection("Movement");
			AddAttribute(_UnitConfiguration.MovementRules.UniqueKey);
			if (_UnitConfiguration.CannotUseRoadMovementWithOversizedPassenger)
				AddAttribute("Oversized Passenger Penalty");
			if (Math.Abs(_UnitConfiguration.OversizedPassengerMovementMultiplier - 1) > float.Epsilon)
				AddAttribute(
					string.Format(
						"Oversize Passenger Multiplier {0}", _UnitConfiguration.OversizedPassengerMovementMultiplier));
		}

		protected void AddMountedInfantryAttributes()
		{
			if (_UnitConfiguration.DismountAs != null || _UnitConfiguration.CanRemount)
				AddSection("Mounted Infantry");
			if (_UnitConfiguration.DismountAs != null)
				AddAttribute(ObjectDescriber.Describe(_UnitConfiguration.DismountAs));
			if (_UnitConfiguration.CanRemount) AddAttribute("Can Re-mount");
		}
	}
}
