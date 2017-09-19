using System;

using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface Order : Serializable
	{
		Army Army { get; }
		OrderStatus Execute(Random Random);
	}
}
