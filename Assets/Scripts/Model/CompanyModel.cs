using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyModel : MonoBehaviour {

	public string companyID;
	public string name;
	[SerializeField]
	public Dictionary<string, object> employees;

	public CompanyModel (string companyID, string name)
	{
		this.companyID = companyID;
		this.name = name;
		employees = new Dictionary<string, object> ();
	}
}
