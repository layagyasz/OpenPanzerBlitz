using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface MapConfiguration : Serializable
	{
		Map GenerateMap(Environment Environment, IdGenerator IdGenerator);
	}
}
