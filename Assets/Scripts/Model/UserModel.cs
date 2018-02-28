using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UserModel {

	public string userID;
	public string name;
	
	[SerializeField]
	public Dictionary<string, object> appoitments;

	public UserModel (string userID, string name)
	{
		this.userID = userID;
		this.name = name;
		appoitments = new Dictionary<string, object> ();
	}

}
