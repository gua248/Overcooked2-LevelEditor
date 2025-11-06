using UnityEngine;

public class TooltipAttribute : PropertyAttribute
{
	public readonly string text;

	public TooltipAttribute(string text)
	{
		this.text = text;
	}
}
