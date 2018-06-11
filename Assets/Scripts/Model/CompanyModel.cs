using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanyModel : UserModel
{
	public string city;
	public string address;
	public string cep;
	[SerializeField]
	public Dictionary<string, object> clients;
	[SerializeField]
	public Dictionary<string, object> employees;
	[SerializeField]
	public List<int> timeToBeginWork;
	[SerializeField]
	public List<int> timeToFinishWork;
	[SerializeField]
	public List<bool> daysOfWork;
	public Dictionary<string, object> servicesProvided;

	public CompanyModel (UserModel user) : base (user.userID, user.name, user.phone, Constants.UserType.Company)
	{
		
		this.daysOfWork = new List<bool> ();
		timeToBeginWork = new List<int> ();
		timeToFinishWork = new List<int> ();
		clients = new Dictionary<string, object> ();
		employees = new Dictionary<string, object> ();
		servicesProvided = new Dictionary<string, object> ();
	}

	public CompanyModel (UserModel user, string city, string address, string cep, int[] timeToBegin, int[] timeToFinish, bool[] daysOfWork) : base (user.userID, user.name, user.phone, Constants.UserType.Company)
	{
		this.city = city;
		this.address = address;
		this.cep = cep;
		//Start at sunday
		this.daysOfWork = new List<bool> (daysOfWork);
		timeToBeginWork = new List<int> (timeToBegin);
		timeToFinishWork = new List<int> (timeToFinish);
		clients = new Dictionary<string, object> ();
		employees = new Dictionary<string, object> ();
	}

}
