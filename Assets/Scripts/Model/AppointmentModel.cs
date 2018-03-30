using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class AppointmentModel
{
	public string appointmentID;
	public string data;
	public int hour;
	public int minute;
	public string userID;
	public string userName;
	public string responsableID;
	public string responsibleName;
	public string description;
	public int durationInMinutes;
	public bool isNew;

	public AppointmentModel (string data, int hour, int minute, string userID, string userName, string responsableID, string responsibleName, string description = "", int durationInMinutes = 30)
	{
		this.data = data;
		this.hour = hour;
		this.minute = minute;
		this.userID = userID;
		this.userName = userName;
		this.responsableID = responsableID;
		this.responsibleName = responsibleName;
		this.description = description;
		this.durationInMinutes = durationInMinutes;
		this.isNew = true;
	}


	public AppointmentModel (DateTime data, string userID, string userName, string responsableID, string responsibleName, string description = "", int durationInMinutes = 30)
	{
		this.data = data.ToString (Constants.dateformat);
		this.hour = data.Hour;
		this.minute = data.Minute;
		this.userID = userID;
		this.userName = userName;
		this.responsableID = responsableID;
		this.responsibleName = responsibleName;
		this.description = description;
		this.durationInMinutes = durationInMinutes;
		this.isNew = true;
	}

}
