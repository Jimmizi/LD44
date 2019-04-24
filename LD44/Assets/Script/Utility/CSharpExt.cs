using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public static class CSharpExt
{
	/// <summary>
	/// Take a float and normalise it between a min and max value.
	/// </summary>
	/// <param name="data">automatically filled by the calling variable</param>
	/// <param name="min">the min amount to scale to 0.0f</param>
	/// <param name="max">the max amount to scale to 1.0f</param>
	/// <returns>a value between 0.0f and 1.0f. Will return closer to 0.0f if
	///     closer to min, closer to 1.0f if closer to max</returns>
	public static float Normalise(this float data, float min, float max)
	{
		return (data - min) / (max - min);
	}

	/// <summary>
	/// Adds a unique type to a list, will reject it if the type is already present. Same type values can be different
	///     (e.g. class with different variable values, integer with different number) and will still reject it 
	/// </summary>
	/// <param name="data">automatically filled by calling list</param>
	/// <param name="toAdd">the type to try and add</param>
	/// <returns>true if added successfully, false if type already exists.</returns>
	public static bool AddUniqueType<T>(this List<T> data, T toAdd)
	{
		if (data.Any(x => x.GetType() == toAdd.GetType()))
		{
			return false;
		}

		data.Add(toAdd);
		return true;
	}
}
