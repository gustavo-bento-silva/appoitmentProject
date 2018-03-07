using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyModel : UserModel
{
	public string city;
	public string address;
	public string cep;
	[SerializeField]
	public Dictionary<string, object> employees;

	public CompanyModel (UserModel user, string city, string address, string cep) : base (user.userID, user.name, user.phone, Constants.UserType.Company)
	{
		this.city = city;
		this.address = address;
		this.cep = cep;
		employees = new Dictionary<string, object> ();
	}
}
