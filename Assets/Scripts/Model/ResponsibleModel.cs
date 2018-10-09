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
	public List<float> timeToBeginWork;
	[SerializeField]
	public List<float> timeToFinishWork;
	[SerializeField]
	public List<bool> daysOfWork;
	[SerializeField]
	public List<BlockDay> blockDayList;
	[SerializeField]
	public LunchTime lunchTime;

	public ResponsibleModel(UserModel user) : base(user.userID, user.name, user.phone, Constants.UserType.Responsible)
	{
		this.companyID = "";
		servicesProvided = new Dictionary<string, object>();
		this.daysOfWork = new List<bool>(new bool[] { false, true, true, true, true, true, true });
		timeToBeginWork = new List<float>(new float[] { 8f, 8f, 8f, 8f, 8f, 8f, 8f });
		timeToFinishWork = new List<float>(new float[] { 17, 17, 17, 17, 17, 17, 17 });
		blockDayList = new List<BlockDay>();
		lunchTime = new LunchTime(0, 0);
	}

	public ResponsibleModel(UserModel user, string companyID, List<ServicesProvidedModel> services, List<bool> daysWorked, List<float> timeToBeginWork, List<float> timeToFinishWork, LunchTime lunchTime) : base(user.userID, user.name, user.phone, Constants.UserType.Responsible)
	{
		this.companyID = companyID;
		servicesProvided = new Dictionary<string, object>();
		servicesProvided = services.ToDictionary(x => x.serviceID, x => (object)x);
		this.daysOfWork = daysWorked;
		this.timeToBeginWork = timeToBeginWork;
		this.timeToFinishWork = timeToFinishWork;
		blockDayList = new List<BlockDay>();
		lunchTime = lunchTime;
	}
}

[System.Serializable]
public class BlockDay
{
	public string id;
	public string data;

	public BlockDay(DateTime mData)
	{
		id = "";
		data = mData.ToString(Constants.dateformat);
	}
}

[System.Serializable]
public class LunchTime
{
	public float initTime;
	public float endTime;

	public LunchTime()
	{
	}

	public LunchTime(float initTime, float endTime)
	{
		this.initTime = initTime;
		this.endTime = endTime;
	}
}
