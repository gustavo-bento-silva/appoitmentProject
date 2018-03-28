using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserModel
{

	public string userID;
	public string name;
	public string phone;
	public string userType;

	public Dictionary<string, object> appoitments;
	public Dictionary<string, object> messages;

	public UserModel (string userID, string name, string phone, Constants.UserType userType = Constants.UserType.User)
	{
		this.userID = userID;
		this.name = name;
		this.phone = phone;
		this.userType = userType.ToString ();
		appoitments = new Dictionary<string, object> ();
		messages = new Dictionary<string, object> ();
	}

}
