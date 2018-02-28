using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserModel {

	public string userID;
	public string name;
	public string userType;
	
	[SerializeField]
	public Dictionary<string, object> appoitments;

	public UserModel (string userID, string name, Constants.UserType userType = Constants.UserType.Client)
	{
		this.userID = userID;
		this.name = name;
		this.userType = userType.ToString();
		appoitments = new Dictionary<string, object> ();
	}

}
