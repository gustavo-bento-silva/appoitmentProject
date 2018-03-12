﻿using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class AppointmentModel
{
	public string appointmentID;
	public string data;
	public string hour;
	public string minute;
	public string userID;
	public string responsableID;
	public string responsibleName;
	public string description;
	public int durationInMinutes;
	public bool isNew;

	public AppointmentModel (string data, string hour, string minute, string userID, string responsableID, string responsibleName, string description = "", int durationInMinutes = 30)
	{
		this.data = data;
		this.hour = hour;
		this.minute = minute;
		this.userID = userID;
		this.responsableID = responsableID;
		this.responsibleName = responsibleName;
		this.description = description;
		this.durationInMinutes = durationInMinutes;
		this.isNew = true;
	}


	public AppointmentModel (DateTime data, string userID, string responsableID, string responsibleName, string description = "", int durationInMinutes = 30)
	{
		this.data = data.ToString (Constants.dateformat);
		this.hour = data.Hour.ToString ();
		this.minute = data.Minute.ToString ();
		this.userID = userID;
		this.responsableID = responsableID;
		this.responsibleName = responsibleName;
		this.description = description;
		this.durationInMinutes = durationInMinutes;
		this.isNew = true;
	}

}
