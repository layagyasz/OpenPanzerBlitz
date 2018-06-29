using Cardamom.Interface.Items;

namespace PanzerBlitz
{
	public class UnitInfoPane : UnitInfoPaneBase
	{
		public UnitInfoPane(Unit Unit, UnitConfigurationRenderer Renderer)
			: base(Unit.Configuration, Unit.Army.Configuration.Faction, Renderer)
		{
			_Info.Add(new Button("unit-info-header") { DisplayedString = ObjectDescriber.Describe(Unit) });

			AddBasicAttributes();
			AddComposition();
			AddCombatCapabilities();
			AddSightCapabilities();
			AddTransportCapabilities();
			AddMovementAttributes();
			AddMountedInfantryAttributes();
		}
	}
}
