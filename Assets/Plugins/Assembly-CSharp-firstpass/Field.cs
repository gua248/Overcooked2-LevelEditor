public class Field
{
	private readonly string parameter;

	public Field(string parameter)
	{
		this.parameter = parameter;
	}

	public override string ToString()
	{
		return parameter;
	}
}
