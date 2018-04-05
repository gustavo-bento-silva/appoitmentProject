using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using System.Globalization;

public class ScheduleDetailPageController : PageController
{
	public RectTransform scrollViewContent;
	public Transform cellPrefabTransform;

	public Text appointmentData;

	int appointmentDay;
	int appointmentMonth;
	int appointmentYear;

	int limit;
	int begin;

	List<GameObject> cellList = new List<GameObject> ();

	DateTime dt;

	void Start ()
	{
		Loading = true;
		GetAppointmentList ();
	}

	void GetAppointmentList ()
	{
		DataManager.GetResponsibleAppointments (delegate() {
			Initilize ();
		}, delegate(string error) {
			Loading = false;
			Error = true;
		});
	}

	void Initilize ()
	{
		UpdateTextData ();
		var dtNow = DateTime.Now;
		dt = new DateTime (appointmentYear, appointmentMonth, appointmentDay, DataManager.currentResponsible.timeToBeginWork [(int)dtNow.DayOfWeek], 0, 0);
		InitializeScheduleTime ();
		StartCoroutine (OnButtonClick ());
	}

	void Update ()
	{
	}

	void UpdateTextData ()
	{
		appointmentDay = DataManager.dateNewAppointment.Day;
		appointmentMonth = DataManager.dateNewAppointment.Month;
		appointmentYear = DataManager.dateNewAppointment.Year;
	
		appointmentData.text = string.Format ("Dia {0}/{1}/{2} \nProfissional: {3} \nServiço: {4}", appointmentDay, appointmentMonth, 
			appointmentYear, DataManager.currentResponsible.name, DataManager.currentservice.name);
	}

	List<AppointmentModel> CreateApoointmentList ()
	{
		var appointmentList = new List<AppointmentModel> ();
		appointmentList.Add (new AppointmentModel (new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
			DataManager.currentResponsible.timeToBeginWork [(int)DateTime.Now.DayOfWeek], 30, 0), "teste", "Teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	IEnumerator OnButtonClick ()
	{
		yield return new WaitForSeconds (1f);
		cellList.ForEach (x => x.transform.SetParent (scrollViewContent, false));
		ReadjustScrollSize (limit);
		Loading = false;
	}

	void ReadjustScrollSize (int size)
	{
		scrollViewContent.anchorMax = new Vector2 (1, 1);
		scrollViewContent.anchorMin = new Vector2 (0, 1);

		scrollViewContent.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefabTransform).rect.height * (size + 1));

		scrollViewContent.offsetMin = new Vector2 (0, -number);
	}

	void InitializeScheduleTime ()
	{
		var dayOfWeek = (int)DataManager.dateNewAppointment.DayOfWeek;
		limit = DataManager.currentResponsible.timeToFinishWork [dayOfWeek];
		begin = DataManager.currentResponsible.timeToBeginWork [dayOfWeek];
		List<AppointmentModel> appointmentList = new List<AppointmentModel> ();
		var isOneInOneHour = PlayerPreferences.oneInOneHour;
		int index = 0;
		bool isResponsible = false; 
		var newAppointmentDate = new DateTime (appointmentYear, appointmentMonth, appointmentDay);
		CultureInfo provider = new CultureInfo ("pt-BR");

		if (DataManager.currentResponsible.userID == DataManager.currentUser.userID) {
			isResponsible = true;
		}

		DataManager.responsibleAppointmentList.ForEach (x => {
			if (DateTime.ParseExact (x.data, Constants.dateformat, provider) == newAppointmentDate) {
				appointmentList.Add (x);
			}
		});

		if (appointmentList != null)
			appointmentList.Sort ((first, second) => ((new DateTime (appointmentYear, appointmentMonth, appointmentDay, first.hour, first.minute, 0)).CompareTo ((new DateTime (appointmentYear, appointmentMonth, appointmentDay, second.hour, second.minute, 0)))));

		if (!isOneInOneHour) {
			limit = (limit - begin) * 2;
		}
		var into = false;

		for (var i = 0; i < limit; i++) {
			if (appointmentList != null && index < appointmentList.Count) {
				var initAppointmentDateTime = (new DateTime (appointmentYear, appointmentMonth, appointmentDay, appointmentList [index].hour, appointmentList [index].minute, 0));
				var finishAppointmentDateTime = (new DateTime (appointmentYear, appointmentMonth, appointmentDay, appointmentList [index].hour, appointmentList [index].minute, 0)).AddMinutes (appointmentList [index].durationInMinutes);

				if (into) {
					if (finishAppointmentDateTime.CompareTo (dt) > 0) {
						var description = "";
						if (isResponsible) {
							description = appointmentList [index].userName;
						} else {
							description = "Ocupado";
						}
						var cell = DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), description, false);
						cell.GetComponent<Button> ().interactable = false;
						cellList.Add (cell);
						into = true;
						dt = dt.AddMinutes (isOneInOneHour ? 60 : 30);
					} else {
						into = false;
						limit++;
						index++;
					}
				} else {
					if (initAppointmentDateTime.CompareTo (dt) == 0) {
						var description = "";
						if (isResponsible) {
							description = appointmentList [index].userName;
						} else {
							description = "Ocupado";
						}
						var cell = DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), description, false);
						cell.GetComponent<Button> ().interactable = false;
						cellList.Add (cell);
						into = true;
					} else {
						cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre"));
					}
					dt = dt.AddMinutes (isOneInOneHour ? 60 : 30);
				}
			} else {
				cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre"));
				dt = dt.AddMinutes (isOneInOneHour ? 60 : 30);
			}
		}
	}
}