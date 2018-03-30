using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System.Globalization;
using System;

public class MyAppointmentsPageController : PageController
{

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;

	List <GameObject> appointmentsCell = new List<GameObject> ();
	List<AppointmentModel> appointmentsList = new List<AppointmentModel> ();

	void Start ()
	{
		CheckAppointments ();
	}

	void Update ()
	{

	}

	void CheckAppointments ()
	{
		Loading = true;
		if (DataManager.currentUser != null) {
			if (DataManager.currentUser.appoitments.Count >= 1) {
				nullListMessage.SetActive (false);
				FillList ();
			} else {
				nullListMessage.SetActive (true);
				Loading = false;
			}
		} else {
			nullListMessage.SetActive (true);
			Loading = false;
		}
	}

	void FillList ()
	{
		foreach (var appointmentKey in DataManager.currentUser.appoitments.Keys) {
			appointmentsList.Add ((AppointmentModel)DataManager.currentUser.appoitments [appointmentKey]);
		}

		CultureInfo provider = new CultureInfo ("pt-BR");
		appointmentsList.Sort ((first, second) => ((DateTime.ParseExact (first.data, Constants.dateformat, provider).Add (new TimeSpan (first.hour, first.minute, 0))).CompareTo ((DateTime.ParseExact (second.data, Constants.dateformat, provider).Add (new TimeSpan (second.hour, second.minute, 0))))));

//		appointmentsList.Sort ((first, second) => (first.data.CompareTo (second.data)));
		if (DataManager.currentUser.userType == Constants.UserType.Responsible.ToString ()) {
			appointmentsList.ForEach (x => appointmentsCell.Add (MyAppointmentController.Instantiate (cellPrefab, string.Format ("{0} \n {1}:{2}h", x.data, x.hour, x.minute.ToString ("00")), x.description, x.userName, x)));
		} else {
			appointmentsList.ForEach (x => appointmentsCell.Add (MyAppointmentController.Instantiate (cellPrefab, string.Format ("{0} \n {1}:{2}h", x.data, x.hour, x.minute.ToString ("00")), x.description, x.responsibleName, x)));
		}
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		appointmentsCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (appointmentsCell.Count);
		DataManager.SetMyAppointmentsAsRead ();
		Loading = false;
	}

	void ReadjustScrollSize (int size)
	{
		scrollContentList.anchorMax = new Vector2 (1, 1);
		scrollContentList.anchorMin = new Vector2 (0, 1);

		scrollContentList.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefab).rect.height * (size + 1));

		scrollContentList.offsetMin = new Vector2 (0, -number);
	}
}
