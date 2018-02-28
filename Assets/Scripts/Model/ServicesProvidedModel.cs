using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ServicesProvidedModel
{
	public string serviceID;
	public string name;
	public float duration;

	public ServicesProvidedModel(string name, float duration)
	{
		this.name = name;
		this.duration = duration;
	}
}
