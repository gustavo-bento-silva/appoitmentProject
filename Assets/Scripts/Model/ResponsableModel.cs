using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResponsableModel : MonoBehaviour {
	
	public string responsableID;
	public string name;
	[SerializeField]
	public Dictionary<string, object> appoitments;

	public ResponsableModel (string responsableID, string name)
	{
		this.responsableID = responsableID;
		this.name = name;
		appoitments = new Dictionary<string, object> ();
	}
}
