using UnityEngine;
using System.Collections;

public class AppointmentModel : MonoBehaviour {

	public string data;
	public string userID;
	public string responsableID;
	public string time;

	public AppointmentModel(string data, string userID, string responsableID, string time){
		this.data = data;
		this.userID = userID;
		this.responsableID = responsableID;
		this.time = time;
	}

}
