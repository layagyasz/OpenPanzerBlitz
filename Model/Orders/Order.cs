using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Order : Serializable
	{
		Army Army { get; }
		bool Execute(Random Random);
	}
}
