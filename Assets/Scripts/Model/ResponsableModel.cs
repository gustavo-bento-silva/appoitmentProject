using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResponsableModel {
	
	[SerializeField]
	public string responsableID;
	public string name;
	
//	[NonSerialized]
	public Dictionary<string, object> appoitments;

	public ResponsableModel (string responsableID, string name)
	{
		this.responsableID = responsableID;
		this.name = name;
		appoitments = new Dictionary<string, object> ();
	}
}
