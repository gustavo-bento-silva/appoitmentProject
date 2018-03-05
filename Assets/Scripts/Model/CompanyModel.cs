using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyModel : UserModel
{

	[SerializeField]
	public Dictionary<string, object> employees;
	[SerializeField]
	public Dictionary<string, object> servicesProvided;

	public CompanyModel (UserModel user) : base (user.userID, user.name, Constants.UserType.Company)
	{
		employees = new Dictionary<string, object> ();
		servicesProvided = new Dictionary<string, object> ();
	}
}
