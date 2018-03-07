using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResponsibleModel : UserModel
{
	public Dictionary<string, object> servicesProvided;

	public ResponsibleModel (UserModel user) : base (user.userID, user.name, user.phone, Constants.UserType.Responsable)
	{
		servicesProvided = new Dictionary<string, object> ();
	}
		
}
