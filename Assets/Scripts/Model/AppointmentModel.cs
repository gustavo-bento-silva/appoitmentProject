using System;
using UnityEngine;
using System.Collections;

public class AppointmentModel : MonoBehaviour {

	public DateTime data;
	public string userID;
	public string responsableID;
	public string description;

	public AppointmentModel(DateTime data, string userID, string responsableID, string description = ""){
		this.data = data;
		this.userID = userID;
		this.responsableID = responsableID;
		this.description = description;
	}

}
