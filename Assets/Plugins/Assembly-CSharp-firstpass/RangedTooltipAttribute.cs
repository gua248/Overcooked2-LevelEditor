using UnityEngine;

public class RangedTooltipAttribute : PropertyAttribute
{
	public readonly float min;

	public readonly float max;

	public readonly string text;

	public RangedTooltipAttribute(string text, float min, float max)
	{
		this.text = text;
		this.min = min;
		this.max = max;
	}
}
