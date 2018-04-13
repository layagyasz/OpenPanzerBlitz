namespace PanzerBlitz
{
	public class UnitAssignment
	{
		public readonly Unit Subject;
		public readonly Unit Object;
		public readonly UnitAssignmentType AssignmentType;

		public UnitAssignment(Unit Subject, Unit Object, UnitAssignmentType AssignmentType)
		{
			this.Subject = Subject;
			this.Object = Object;
			this.AssignmentType = AssignmentType;
		}

		public override string ToString()
		{
			return string.Format(
				"[UnitAssignment: Subject={0}, Object={1}, AssignmentType={2}]", Subject, Object, AssignmentType);
		}
	}
}
