using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResponsableModel : UserModel
{
	[SerializeField]
	public Dictionary<string, object> servicesProvided;
	

	public ResponsableModel(string responsibleId, string name) : base(responsibleId, name, Constants.UserType.Responsable)
	{
		servicesProvided = new Dictionary<string, object> ();
	}
		
}
