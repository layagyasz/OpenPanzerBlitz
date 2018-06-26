using System;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class UnitInfoPane : Pane
	{
		public EventHandler<EventArgs> OnClose;

		Button _CloseButton = new Button("unit-info-close-button") { DisplayedString = "X" };
		UnitView _UnitView;
		ScrollCollection<ClassedGuiItem> _Info =
			new ScrollCollection<ClassedGuiItem>("unit-info-display") { Position = new Vector2f(0, 208) };

		public UnitInfoPane(Unit Unit, UnitConfigurationRenderer Renderer)
			: base("unit-info-pane")
		{
			_CloseButton.Position = new Vector2f(Size.X - _CloseButton.Size.X - LeftPadding.X * 2, 0);
			_CloseButton.OnClick += HandleClose;

			_UnitView = new UnitView(Unit, Renderer, 192, false);
			_UnitView.Position = new Vector2f(144, 96);
			_Info.Add(new Button("unit-info-header") { DisplayedString = ObjectDescriber.Describe(Unit) });

			Add(_CloseButton);
			Add(_UnitView);
			Add(_Info);

			AddBasicAttributes(Unit);
			AddComposition(Unit);
			AddCombatCapabilities(Unit);
			AddSightCapabilities(Unit);
			AddTransportCapabilities(Unit);
			AddMovementAttributes(Unit);
			AddMountedInfantryAttributes(Unit);
		}

		void HandleClose(object Sender, EventArgs E)
		{
			if (OnClose != null) OnClose(this, EventArgs.Empty);
		}

		void AddSection(string Section)
		{
			_Info.Add(new Button("unit-info-header-2")
			{
				DisplayedString = Section
			});
		}

		void AddAttribute(string Attribute)
		{
			_Info.Add(new Button("unit-info-attribute")
			{
				DisplayedString = Attribute
			});
		}

		void AddBasicAttributes(Unit Unit)
		{
			AddSection("Basic Attributes");
			AddAttribute(string.Format(
				"Unit Class - {0}", ObjectDescriber.Describe(Unit.Configuration.UnitClass)));
			AddAttribute(
				string.Format(
					"Weapon Class - {0}", ObjectDescriber.Describe(Unit.Configuration.PrimaryWeapon.WeaponClass)));
			AddAttribute(string.Format("Attack - {0}", Unit.Configuration.PrimaryWeapon.Attack));
			AddAttribute(string.Format("Range - {0}", Unit.Configuration.PrimaryWeapon.Range));
			AddAttribute(string.Format("Defense - {0}", Unit.Configuration.Defense));
			AddAttribute(
				string.Format(
					"Movement - {0}",
					Unit.Configuration.HasUnlimitedMovement() ? "Unlimited" : Unit.Configuration.Movement.ToString()));
			AddAttribute(string.Format("Point Value - {0}", Unit.GetPointValue()));
		}

		void AddComposition(Unit Unit)
		{
			AddSection("Composition");
			AddAttribute(string.Format("Weight - {0}", Unit.Configuration.UnitWeight));
			if (Unit.Configuration.IsVehicle) AddAttribute("Vehicular");
			if (Unit.Configuration.IsArmored) AddAttribute("Armored");
			if (Unit.Configuration.WrecksAs != null)
				AddAttribute(string.Format("Wrecks As - {0}", ObjectDescriber.Describe(Unit.Configuration.WrecksAs)));
		}

		void AddCombatCapabilities(Unit Unit)
		{
			if (Unit.Configuration.IsEngineer
				|| Unit.Configuration.IsParatroop
				|| Unit.Configuration.IsCommando
				|| Unit.Configuration.CanDirectFire
				|| Unit.Configuration.CanIndirectFire
				|| Unit.Configuration.CanOverrun
				|| Unit.Configuration.CanOnlyOverrunUnarmored
				|| Unit.Configuration.CanCloseAssault
				|| Unit.Configuration.CanOnlySupportCloseAssault
				|| Unit.Configuration.CanAntiAircraft
				|| Unit.Configuration.PrimaryWeapon.CanDoubleRange
				|| Unit.Configuration.CanClearMines
				|| Unit.Configuration.CanPlaceMines
				|| Unit.Configuration.CanPlaceBridges
				|| Unit.Configuration.InnatelyClearsMines
				|| Unit.Configuration.ImmuneToMines
				|| Unit.Configuration.HasLowProfile)
				AddSection("Combat Capabilities");
			if (Unit.Configuration.IsEngineer) AddAttribute("Engineers");
			if (Unit.Configuration.IsParatroop) AddAttribute("Paratroops");
			if (Unit.Configuration.IsCommando) AddAttribute("Commandos");
			if (Unit.Configuration.CanDirectFire) AddAttribute("Direct Fire");
			if (Unit.Configuration.CanIndirectFire) AddAttribute("Indirect Fire");
			if (Unit.Configuration.CanOverrun) AddAttribute("Overrun");
			if (Unit.Configuration.CanOnlyOverrunUnarmored) AddAttribute("Overrun Un-armored");
			if (Unit.Configuration.CanCloseAssault) AddAttribute("Close Assault");
			if (Unit.Configuration.CanOnlySupportCloseAssault) AddAttribute("Close Assault Support");
			if (Unit.Configuration.CanAntiAircraft) AddAttribute("Anti-Aircraft");
			if (Unit.Configuration.PrimaryWeapon.CanDoubleRange) AddAttribute("Can Double Range");
			if (Unit.Configuration.CanClearMines) AddAttribute("Can Clear Mines");
			if (Unit.Configuration.CanPlaceMines) AddAttribute("Can Place Mines");
			if (Unit.Configuration.CanPlaceBridges) AddAttribute("Can Place Bridges");
			if (Unit.Configuration.InnatelyClearsMines) AddAttribute("Innately Clears Mines");
			if (Unit.Configuration.ImmuneToMines) AddAttribute("Immune To Mines");
			if (Unit.Configuration.HasLowProfile) AddAttribute("Low Profile");
		}

		void AddSightCapabilities(Unit Unit)
		{
			if (Unit.Configuration.SightRange > 0
				|| Unit.Configuration.SpotRange > 0
				|| Unit.Configuration.CanReveal)
				AddSection("Sight Capabilities");
			if (Unit.Configuration.SightRange > 0)
				AddAttribute(string.Format("Sight Range - {0}", Unit.Configuration.SightRange));
			if (Unit.Configuration.SpotRange > 0)
				AddAttribute(string.Format("Spot Range - {0}", Unit.Configuration.SpotRange));
			if (Unit.Configuration.CanReveal) AddAttribute("Can Reveal");
		}

		void AddTransportCapabilities(Unit Unit)
		{
			if (Unit.Configuration.IsPassenger
				|| Unit.Configuration.IsOversizedPassenger
				|| Unit.Configuration.IsCarrier
				|| Unit.Configuration.UnloadsWhenDisrupted
				|| Unit.Configuration.CanOnlyCarryLight
				|| Unit.Configuration.CanOnlyCarryInfantry
				|| Unit.Configuration.CanCarryInWater)
				AddSection("Transport Capabilities");
			if (Unit.Configuration.IsPassenger) AddAttribute("Passenger");
			if (Unit.Configuration.IsOversizedPassenger) AddAttribute("Oversized Passenger");
			if (Unit.Configuration.IsCarrier) AddAttribute("Carrier");
			if (Unit.Configuration.UnloadsWhenDisrupted) AddAttribute("Unloads When Disrupted");
			if (Unit.Configuration.CanOnlyCarryLight) AddAttribute("Carry Only Light Units");
			if (Unit.Configuration.CanOnlyCarryInfantry) AddAttribute("Carry Only Infantry");
			if (Unit.Configuration.CanCarryInWater) AddAttribute("Can Carry In Water");
		}

		void AddMovementAttributes(Unit Unit)
		{
			AddSection("Movement");
			AddAttribute(Unit.Configuration.MovementRules.UniqueKey);
			if (Unit.Configuration.CannotUseRoadMovementWithOversizedPassenger)
				AddAttribute("Oversized Passenger Penalty");
			if (Math.Abs(Unit.Configuration.OversizedPassengerMovementMultiplier - 1) > float.Epsilon)
				AddAttribute(
					string.Format(
						"Oversize Passenger Multiplier {0}", Unit.Configuration.OversizedPassengerMovementMultiplier));
		}

		void AddMountedInfantryAttributes(Unit Unit)
		{
			if (Unit.Configuration.DismountAs != null || Unit.Configuration.CanRemount)
				AddSection("Mounted Infantry");
			if (Unit.Configuration.DismountAs != null)
				AddAttribute(ObjectDescriber.Describe(Unit.Configuration.DismountAs));
			if (Unit.Configuration.CanRemount) AddAttribute("Can Re-mount");
		}
	}
}
