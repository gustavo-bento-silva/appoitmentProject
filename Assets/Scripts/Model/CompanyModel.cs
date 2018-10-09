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
	public List<float> timeToBeginWork;
	[SerializeField]
	public List<float> timeToFinishWork;
	[SerializeField]
	public List<bool> daysOfWork;
	public Dictionary<string, object> servicesProvided;

	public CompanyModel(UserModel user) : base(user.userID, user.name, user.phone, Constants.UserType.Company)
	{

		this.daysOfWork = new List<bool>();
		timeToBeginWork = new List<float>();
		timeToFinishWork = new List<float>();
		clients = new Dictionary<string, object>();
		employees = new Dictionary<string, object>();
		servicesProvided = new Dictionary<string, object>();
	}

	public CompanyModel(UserModel user, string city, string address, string cep, float[] timeToBegin, float[] timeToFinish, bool[] daysOfWork) : base(user.userID, user.name, user.phone, Constants.UserType.Company)
	{
		this.city = city;
		this.address = address;
		this.cep = cep;
		//Start at sunday
		this.daysOfWork = new List<bool>(daysOfWork);
		timeToBeginWork = new List<float>(timeToBegin);
		timeToFinishWork = new List<float>(timeToFinish);
		clients = new Dictionary<string, object>();
		employees = new Dictionary<string, object>();
	}

}
