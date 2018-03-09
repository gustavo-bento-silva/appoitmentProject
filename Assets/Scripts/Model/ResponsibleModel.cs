using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ResponsibleModel : UserModel
{
	public Dictionary<string, object> servicesProvided;
	[SerializeField]
	public List<int> timeToBeginWork;
	[SerializeField]
	public List<int> timeToFinishWork;
	[SerializeField]
	public List<bool> daysOfWork;

	public ResponsibleModel (UserModel user) : base (user.userID, user.name, user.phone, Constants.UserType.Responsable)
	{
		servicesProvided = new Dictionary<string, object> ();
		this.daysOfWork = new List<bool> (new bool[] { false, true, true, true, true, true, true });
		timeToBeginWork = new List<int> (new int[] { 8, 8, 8, 8, 8, 8, 8 });
		timeToFinishWork = new List<int> (new int[] { 17, 17, 17, 17, 17, 17, 17 });
	}
}
