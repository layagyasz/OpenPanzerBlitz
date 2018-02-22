using Cardamom.Serialization;

namespace PanzerBlitz
{
	public interface ChatAction : Serializable
	{
		Player Player { get; }
		bool Apply(Chat Chat);
	}
}
