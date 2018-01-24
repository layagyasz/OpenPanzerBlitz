using System;
using System.Collections.Generic;

using Cardamom.Interface;
using Cardamom.Interface.Items;
using Cardamom.Utilities;

namespace PanzerBlitz
{
	public class ScenarioBuilderArmySection : TableRow
	{
		public EventHandler<ValuedEventArgs<ArmyParameters>> OnParametersChanged;
		public EventHandler<EventArgs> OnRemoved;

		public readonly ArmyBuilder ArmyBuilder;

		Select<Faction> _FactionSelect = new Select<Faction>("scenario-builder-army-section-faction-select");
		TextInput _PointsInput = new TextInput("scenario-builder-army-section-points-input");
		Select<byte> _TeamSelect = new Select<byte>("scenario-builder-army-section-team-select");

		public ScenarioBuilderArmySection(ArmyBuilder ArmyBuilder, IEnumerable<Faction> Factions)
			: base("scenario-builder-army-section")
		{
			this.ArmyBuilder = ArmyBuilder;

			foreach (Faction faction in Factions)
				_FactionSelect.Add(
					new SelectionOption<Faction>("scenario-builder-army-section-faction-select-option")
					{
						DisplayedString = ObjectDescriber.Describe(faction),
						Value = faction
					});
			_FactionSelect.SetValue(i => i.Value == ArmyBuilder.Parameters.Faction);

			_PointsInput.Value = ArmyBuilder.Parameters.Points.ToString();

			for (byte i = 1; i <= 3; ++i)
				_TeamSelect.Add(
					new SelectionOption<byte>("scenario-builder-army-section-team-select-option")
					{
						DisplayedString = i.ToString(),
						Value = i
					});
			_TeamSelect.SetValue(i => i.Value == ArmyBuilder.Parameters.Team);

			Button removeButton = new Button("scenario-builder-army-section-remove-button") { DisplayedString = "X" };

			_FactionSelect.OnChange += HandleChange;
			_PointsInput.OnLeave += HandleChange;
			_TeamSelect.OnChange += HandleChange;
			removeButton.OnClick += HandleRemove;

			Add(new GuiContainer<GuiItem>("scenario-builder-army-section-faction-cell") { _FactionSelect });
			Add(new GuiContainer<GuiItem>("scenario-builder-army-section-points-cell") { _PointsInput });
			Add(new GuiContainer<GuiItem>("scenario-builder-army-section-team-cell") { _TeamSelect });
			Add(removeButton);
		}

		public bool Validate()
		{
			try
			{
				uint points = Convert.ToUInt32(_PointsInput.Value);
				return points > 0;
			}
			catch (Exception e)
			{
				return false;
			}
		}

		void HandleChange(object Sender, EventArgs E)
		{
			ArmyParameters parameters = null;
			try
			{
				uint points = Convert.ToUInt32(_PointsInput.Value);
				parameters =
					new ArmyParameters(
						_FactionSelect.Value.Value, points, _TeamSelect.Value.Value, ArmyBuilder.Parameters.Parameters);
			}
			catch (Exception e) { }
			if (parameters != null && OnParametersChanged != null)
				OnParametersChanged(this, new ValuedEventArgs<ArmyParameters>(parameters));
		}

		void HandleRemove(object Sender, EventArgs E)
		{
			if (OnRemoved != null) OnRemoved(this, EventArgs.Empty);
		}
	}
}
