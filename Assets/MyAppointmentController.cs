using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;
using System.Globalization;
using System;

public class MyAppointmentController : MonoBehaviour
{

	public Text time;
	public Text day;
	public Text month;
	public Text description;
	public GameObject removeButton;
	public AppointmentModel appointment;


	public void OnRemoveClick ()
	{
		var dict = new Dictionary<string, object> ();
		dict.Add (appointment.appointmentID, appointment);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.ConfirmDeleteAppointmentPopup, dict);

	}

	public static GameObject Instantiate (Transform CellPrefabTransform, string day, string month, string time, string service, string responsible, AppointmentModel appointment)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myAppointmentController = go.GetComponent<MyAppointmentController> ();
		myAppointmentController.description.text = string.Format ("{0}\n{1}", responsible, service);
		myAppointmentController.day.text = day;
		myAppointmentController.month.text = month;
		myAppointmentController.time.text = time;
		myAppointmentController.appointment = appointment;
		CultureInfo provider = new CultureInfo ("pt-BR");
		var appointmentDate = DateTime.ParseExact (appointment.data, Constants.dateformat, provider);
		var dtNow = DateTime.Now;
		TimeSpan timeSpan = appointmentDate - dtNow;

		if (timeSpan.Days < 0) {
			if (timeSpan.Hours > -1) {
				myAppointmentController.removeButton.SetActive (false);
			}
		}
		return go;
	}


}
