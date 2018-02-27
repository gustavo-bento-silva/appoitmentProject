using System;
using UnityEngine;
using System.Collections;

public class AppointmentModel : MonoBehaviour {

	[SerializeField]
	public DateTime data;
	public string userID;
	public string responsableID;
	public string description;
	public int durationInMinutes;

	public AppointmentModel(DateTime data, string userID, string responsableID, string description = "", int durationInMinutes = 30){
		this.data = data;
		this.userID = userID;
		this.responsableID = responsableID;
		this.description = description;
		this.durationInMinutes = durationInMinutes;
	}

}
