using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ResponsibleModel : UserModel
{
	public string companyID;
	public Dictionary<string, object> servicesProvided;
	[SerializeField]
	public List<int> timeToBeginWork;
	[SerializeField]
	public List<int> timeToFinishWork;
	[SerializeField]
	public List<bool> daysOfWork;

	public ResponsibleModel (UserModel user, string companyID) : base (user.userID, user.name, user.phone, Constants.UserType.Responsible)
	{
		this.companyID = companyID;
		servicesProvided = new Dictionary<string, object> ();
		this.daysOfWork = new List<bool> (new bool[] { false, true, true, true, true, true, true });
		timeToBeginWork = new List<int> (new int[] { 8, 8, 8, 8, 8, 8, 8 });
		timeToFinishWork = new List<int> (new int[] { 17, 17, 17, 17, 17, 17, 17 });
	}

	public ResponsibleModel (UserModel user, string companyID, List<ServicesProvidedModel> services, List<bool> daysWorked, List<int> timeToBeginWork, List<int> timeToFinishWork) : base (user.userID, user.name, user.phone, Constants.UserType.Responsible)
	{
		this.companyID = companyID;
		servicesProvided = new Dictionary<string, object> ();
		servicesProvided = services.ToDictionary (x => x.serviceID, x => (object)x);
		this.daysOfWork = daysWorked;
		this.timeToBeginWork = timeToBeginWork;
		this.timeToFinishWork = timeToFinishWork;
	}
}
