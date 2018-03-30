using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PageNavFrameWork;

public class MyAppointmentController : MonoBehaviour
{

	public Text time;
	public Text description;
	public AppointmentModel appointment;


	public void OnRemoveClick ()
	{
		var dict = new Dictionary<string, object> ();
		dict.Add (appointment.appointmentID, appointment);
		PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.ConfirmDeleteAppointmentPopup, dict);

	}

	public static GameObject Instantiate (Transform CellPrefabTransform, string time, string service, string responsible, AppointmentModel appointment)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myAppointmentController = go.GetComponent<MyAppointmentController> ();
		myAppointmentController.description.text = string.Format ("{0} \n {1}", responsible, service);
		myAppointmentController.time.text = time;
		myAppointmentController.appointment = appointment;
		return go;
	}
}
