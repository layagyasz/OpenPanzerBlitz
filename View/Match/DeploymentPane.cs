using System;
using System.Collections.Generic;

using Cardamom.Interface.Items;
using Cardamom.Utilities;


namespace PanzerBlitz
{
	public class DeploymentPane : Pane
	{
		public EventHandler<EventArgs> OnDeploymentSelected;

		Select<DeploymentPage> _DeploymentPageSelect = new Select<DeploymentPage>("select");
		readonly List<DeploymentPage> _Pages = new List<DeploymentPage>();

		public Deployment SelectedDeployment
		{
			get
			{
				return _DeploymentPageSelect.Value.Value.Deployment;
			}
		}

		public DeploymentPane()
			: base("deployment-pane")
		{
			_DeploymentPageSelect.OnChange += SetActivePage;
			Add(_DeploymentPageSelect);
		}

		public void AddPage(DeploymentPage Page)
		{
			Page.Visible = false;
			_Pages.Add(Page);
			Insert(0, Page);

			var option = new SelectionOption<DeploymentPage>("select-option")
			{
				Value = Page,
				DisplayedString = Page.Deployment.Configuration.UnitGroup.Name
			};
			_DeploymentPageSelect.Add(option);
		}

		void SetActivePage(object sender, ValuedEventArgs<StandardItem<DeploymentPage>> E)
		{
			_Pages.ForEach(i => i.Visible = false);
			E.Value.Value.Visible = true;
			if (OnDeploymentSelected != null) OnDeploymentSelected(null, EventArgs.Empty);
		}
	}
}
