namespace PanzerBlitz
{
	public class IdGenerator
	{
		int _NextId;

		public IdGenerator()
		{
			_NextId = 1;
		}

		public IdGenerator(int InitialId)
		{
			_NextId = InitialId;
		}

		public int GenerateId()
		{
			return _NextId++;
		}
	}
}
