using System;
using UnityEngine;

public static class VectorUtil
{
	public static Vector3 ParseFromKey(string key)
	{
		string[] parts = key.Split('-');
		if (parts.Length != 3)
		{
			throw new FormatException($"Invalid key format: {key}");
		}

		float x = float.Parse(parts[0]);
		float y = float.Parse(parts[1]);
		float z = float.Parse(parts[2]);

		return new Vector3(x, y, z);
	}
}
