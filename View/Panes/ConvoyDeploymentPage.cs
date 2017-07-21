using System;
using System.Linq;

using Cardamom.Interface.Items;

using SFML.Window;

namespace PanzerBlitz
{
	public class ConvoyDeploymentPage : DeploymentPage
	{
		ConvoyDeployment _Deployment;
		UnitConfigurationRenderer _Renderer;
		ScrollCollection<StackView> _Selection;

		public override Deployment Deployment
		{
			get
			{
				return _Deployment;

			}
		}
		public ConvoyDeploymentPage(ConvoyDeployment Deployment, UnitConfigurationRenderer Renderer)
		{
			_Deployment = Deployment;
			_Renderer = Renderer;

			_Selection = new ScrollCollection<StackView>("deployment-select");

			foreach (Unit u in Deployment.Units)
				_Selection.Add(new ConvoyDeploymentSelectionOption(u, _Renderer));

			_Selection.Position = new Vector2f(0, 48);
			Add(_Selection);
		}
	}
}
