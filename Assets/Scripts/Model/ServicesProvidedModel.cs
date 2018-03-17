using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ServicesProvidedModel
{
	public string serviceID;
	public string name;
	public string price;
	public float duration;

	public ServicesProvidedModel (string name, float duration, string price)
	{
		this.name = name;
		this.duration = duration;
		this.price = price;
	}
}
