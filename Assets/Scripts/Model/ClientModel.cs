using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientModel
{

	public string userID;
	public string name;
	public string phone;
	public string userType;
	public int visitToCompany;
	public int visitCount;

	public ClientModel (string userID, string name, string phone, Constants.UserType userType = Constants.UserType.User)
	{
		this.userID = userID;
		this.name = name;
		this.phone = phone;
		this.userType = userType.ToString ();
		this.visitCount = 0;
		this.visitToCompany = 0;
	}
}
