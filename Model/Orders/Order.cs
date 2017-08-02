using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Order : Serializable
	{
		bool Execute(Random Random);
	}
}
