using System;
using System.Collections.Generic;
using System.Linq;

using Cardamom.Interface;
using Cardamom.Interface.Items;

using SFML.Graphics;


namespace PanzerBlitz
{
	public class DeploymentPane : Pane
	{
		public EventHandler<EventArgs> OnDeploymentSelected;

		Select<DeploymentPage> _DeploymentPageSelect = new Select<DeploymentPage>("select");
		List<DeploymentPage> _Pages = new List<DeploymentPage>();

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
			Add(Page);

			SelectionOption<DeploymentPage> option = new SelectionOption<DeploymentPage>("select-option")
			{
				Value = Page,
				DisplayedString = Page.Deployment.Configuration.DisplayName
			};
			_DeploymentPageSelect.Add(option);
		}

		void SetActivePage(object sender, ValueChangedEventArgs<StandardItem<DeploymentPage>> E)
		{
			_Pages.ForEach(i => i.Visible = false);
			E.Value.Value.Visible = true;
			if (OnDeploymentSelected != null) OnDeploymentSelected(null, EventArgs.Empty);
		}
	}
}
