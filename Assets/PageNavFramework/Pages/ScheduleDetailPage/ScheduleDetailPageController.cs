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
		var minutes = 0;
		if (!PlayerPreferences.oneInOneHour) {
			minutes = 30;
		}

		dt = new DateTime (dtNow.Year, dtNow.Month, dtNow.Day, DataManager.currentResponsible.timeToBeginWork [(int)dtNow.DayOfWeek], minutes, 0);
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
	
		appointmentData.text = string.Format ("Dia {0}/{1}/{2} \nProssifional: {3} \nServiço: {4}", appointmentDay, appointmentMonth, 
			appointmentYear, DataManager.currentResponsible.name, DataManager.currentservice.name);
	}

	List<AppointmentModel> CreateApoointmentList ()
	{
		var appointmentList = new List<AppointmentModel> ();
		appointmentList.Add (new AppointmentModel (new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 
			DataManager.currentResponsible.timeToBeginWork [(int)DateTime.Now.DayOfWeek], 30, 0), "teste", "Teste", "Ocupado"));
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
		limit = DataManager.currentResponsible.timeToFinishWork [(int)DataManager.dateNewAppointment.DayOfWeek];
		begin = DataManager.currentResponsible.timeToBeginWork [(int)DataManager.dateNewAppointment.DayOfWeek];
		List<AppointmentModel> appointmentList = new List<AppointmentModel> ();
		var isOneInOneHour = PlayerPreferences.oneInOneHour;
		int index = 0;
		var newAppointmentDate = new DateTime (appointmentYear, appointmentMonth, appointmentDay);
		CultureInfo provider = new CultureInfo ("pt-BR");

		DataManager.responsibleAppointmentList.ForEach (x => {
			if (DateTime.ParseExact (x.data, Constants.dateformat, provider) == newAppointmentDate) {
				appointmentList.Add (x);
			}
		});

		if (appointmentList != null)
			appointmentList.Sort ((first, second) => ((first.data).CompareTo (second.data)));

		if (!isOneInOneHour) {
			limit = (limit - begin) * 2;
		}

		for (var i = 0; i < limit; i++) {
			if (appointmentList != null && index < appointmentList.Count) {
				if (int.Parse (appointmentList [index].hour) == dt.Hour && int.Parse (appointmentList [index].minute) == dt.Minute) {
					cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Ocupado", false));
					index++;
				} else {
					cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre"));
				}
			} else {
				cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre"));
			}
			dt = dt.AddMinutes (isOneInOneHour ? 60 : 30);
		}
	}
}