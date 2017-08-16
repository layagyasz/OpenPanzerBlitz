using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface ChatAction : Serializable
	{
		bool Apply(Chat Chat);
	}
}
