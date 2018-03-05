using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResponsableModel : UserModel
{
	public Dictionary<string, object> servicesProvided;

	public ResponsableModel (UserModel user) : base (user.userID, user.name, Constants.UserType.Responsable)
	{
		servicesProvided = new Dictionary<string, object> ();
	}
		
}
