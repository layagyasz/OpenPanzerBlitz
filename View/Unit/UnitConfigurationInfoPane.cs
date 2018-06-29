using Cardamom.Interface.Items;

namespace PanzerBlitz
{
	public class UnitConfigurationInfoPane : UnitInfoPaneBase
	{
		public UnitConfigurationInfoPane(
			UnitConfiguration UnitConfiguration, Faction Faction, UnitConfigurationRenderer Renderer)
			: base(UnitConfiguration, Faction, Renderer)
		{
			_Info.Add(new Button("unit-info-header") { DisplayedString = ObjectDescriber.Describe(UnitConfiguration) });

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
