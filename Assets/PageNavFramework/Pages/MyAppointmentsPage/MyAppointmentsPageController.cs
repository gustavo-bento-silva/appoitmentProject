using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System.Globalization;
using System;
using UnityEngine.UI;

public class MyAppointmentsPageController : PageController
{

	const string todayNullMessage = "Você não possui nenhum agendamento para hoje!";
	const string weekNullMessage = "Você não possui nenhum agendamento para essa semana!";
	const string allNullMessage = "Você não possui nenhum agendamento.";

	public Transform cellPrefab;
	public RectTransform scrollContentList;
	public GameObject nullListMessage;
	public GameObject imageBar;
	public Text[] tabsText;

	List <GameObject> appointmentsCell = new List<GameObject> ();
	List<AppointmentModel> appointmentsList = new List<AppointmentModel> ();

	float xPositionOffset = 394f;
	int actualPositionIndex = 0;

	public enum AppointmentDate
	{
		Today,
		Week,
		All
	}

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
				ClearAppointments ();
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

	void ClearAppointments ()
	{
		CultureInfo provider = new CultureInfo ("pt-BR");
		foreach (var key in DataManager.currentUser.appoitments.Keys) {
			AppointmentModel appointment = (AppointmentModel)DataManager.currentUser.appoitments [key];
			DateTime date = DateTime.ParseExact (appointment.data, Constants.dateformat, provider).AddHours (appointment.hour).AddMinutes (appointment.minute);
			if (date.CompareTo (DateTime.Now) < 0) {
				DataManager.JustRemoveAppointmentWithouMessage (appointment, delegate() {
				}, delegate(string error) {
					
				});
			}
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
			appointmentsList.ForEach (x => {
				var data = x.data.Split ('-');
				var month = GetMonth (int.Parse (data [1]));
				appointmentsCell.Add (MyAppointmentController.Instantiate (cellPrefab, data [0], month, string.Format ("{0}:{1}h", x.hour, x.minute.ToString ("00")), x.description, x.userName, x));
			});
		} else {
			appointmentsList.ForEach (x => {
				var data = x.data.Split ('-');
				var month = GetMonth (int.Parse (data [1]));
				appointmentsCell.Add (MyAppointmentController.Instantiate (cellPrefab, data [0], month, string.Format ("{0}:{1}h", x.hour, x.minute.ToString ("00")), x.description, x.responsibleName, x));
			});
		}
		StartCoroutine (OnFillList ());
	}

	IEnumerator OnFillList ()
	{
		yield return new WaitForSeconds (1f);
		appointmentsCell.ForEach (x => x.transform.SetParent (scrollContentList, false));
		ReadjustScrollSize (appointmentsCell.Count);
		DataManager.SetMyAppointmentsAsRead ();
		OnTodayClick ();
		Loading = false;
	}

	public void changeTabsColor (int mIndex)
	{
		var index = 0;

		foreach (var text in tabsText) {
			var color = text.color;
			if (index != mIndex) {
				color.a = 0.5f;
			} else {
				color.a = 1;
			}
			text.color = color;
			index++;
		}
	}

	public void OnTodayClick ()
	{
		var mIndex = 0;
		MoveBar (mIndex);
		changeTabsColor (mIndex);
		Filter (AppointmentDate.Today);
	}

	public void OnWeekClick ()
	{
		var mIndex = 1;
		MoveBar (mIndex);
		changeTabsColor (mIndex);
		Filter (AppointmentDate.Week);
	}

	public void OnAllClick ()
	{
		var mIndex = 2;
		MoveBar (mIndex);
		changeTabsColor (mIndex);
		Filter (AppointmentDate.All);
	}

	public void MoveBar (int position)
	{
		var diference = (position - actualPositionIndex);
		actualPositionIndex = position;

		var mPosition = imageBar.transform.localPosition.x + (diference * xPositionOffset);
		iTween.MoveTo (imageBar, iTween.Hash ("x", mPosition, "islocal", true, "time", 0.4, "easetype", iTween.EaseType.easeOutBack));
	}

	public void Filter (AppointmentDate appointmentDate)
	{
		bool hasOneAtLeast = false;
		CultureInfo provider = new CultureInfo ("pt-BR");

		if (appointmentsCell != null && appointmentsCell.Count > 0) {
			switch (appointmentDate) {
			case AppointmentDate.Today:
				nullListMessage.GetComponent<Text> ().text = todayNullMessage;
				appointmentsCell.ForEach (x => {
					DateTime data = DateTime.ParseExact (x.GetComponent<MyAppointmentController> ().appointment.data, Constants.dateformat, provider);
					DateTime now = DateTime.Now;
					if (data.Day == now.Day && data.Month == now.Month && data.Year == now.Year) {
						hasOneAtLeast = true;
						x.SetActive (true);
					} else {
						x.SetActive (false);
					}
				});
				break;

			case AppointmentDate.Week:
				nullListMessage.GetComponent<Text> ().text = weekNullMessage;
				appointmentsCell.ForEach (x => {
					DateTime data = DateTime.ParseExact (x.GetComponent<MyAppointmentController> ().appointment.data, Constants.dateformat, provider);
					if ((data - DateTime.Now).TotalDays <= 7) {
						hasOneAtLeast = true;
						x.SetActive (true);
					} else {
						x.SetActive (false);
					}
				});
				break;

			case AppointmentDate.All:
				nullListMessage.GetComponent<Text> ().text = allNullMessage;
				appointmentsCell.ForEach (x => {
					hasOneAtLeast = true;
					x.SetActive (true);
				});
				break;
			}
		}

		if (!hasOneAtLeast) {
			nullListMessage.SetActive (true);
		} else {
			nullListMessage.SetActive (false);
		}
	}

	String GetMonth (int month)
	{
		switch (month) {
		case 1:
			return "Jan";
		case 2:
			return "Fev";
		case 3:
			return "Mar";
		case 4:
			return "Abr";
		case 5:
			return "Mai";
		case 6:
			return "Jun";
		case 7:
			return "Jul";
		case 8:
			return "Ago";
		case 9:
			return "Set";
		case 10:
			return "Out";
		case 11:
			return "Nov";
		case 12:
			return "Dez";

		}
		return "";
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
