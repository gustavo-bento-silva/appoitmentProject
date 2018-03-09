using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class ScheduleDetailPageController : PageController
{
	public RectTransform scrollViewContent;
	public Transform cellPrefabTransform;

	public Text appointmentData;

	int appointmentDay;
	int appointmentMonth;
	int appointmentYear;

	int limit = 7;
	//PlayerPreferences.GetEndTimeByDay(appointmentDay);
	int begin = 1;
	//PlayerPreferences.initialTime;

	List<GameObject> cellList = new List<GameObject> ();

	DateTime dt;

	void Start ()
	{
//		UpdateTextData ();
		var dtNow = DateTime.Now;
		var minutes = 0;
		if (!PlayerPreferences.oneInOneHour) {
			minutes = 30;
		}

		dt = new DateTime (dtNow.Year, dtNow.Month, dtNow.Day, 7/*DataManager.currentResponsible.timeToBeginWork [(int)dtNow.DayOfWeek]*/, minutes, 0);
		InitializeScheduleTime (CreateApoointmentList ());
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
			9/*DataManager.currentResponsible.timeToBeginWork [(int)DateTime.Now.DayOfWeek]*/, 30, 0), "teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	public void OnButtonClick ()
	{
		cellList.ForEach (x => x.transform.SetParent (scrollViewContent, false));
		ReadjustScrollSize (limit);
	}

	void ReadjustScrollSize (int size)
	{
		scrollViewContent.anchorMax = new Vector2 (1, 1);
		scrollViewContent.anchorMin = new Vector2 (0, 1);

		scrollViewContent.offsetMax = new Vector2 (0, 0);
		var number = (((RectTransform)cellPrefabTransform).rect.height * (size + 1));

		scrollViewContent.offsetMin = new Vector2 (0, -number);
	}

	void InitializeScheduleTime (List<AppointmentModel> appointmentList)
	{
		var isOneInOneHour = PlayerPreferences.oneInOneHour;
		int index = 0;

		if (appointmentList != null)
			appointmentList.Sort ((first, second) => ((first.data).CompareTo (second.data)));

		if (!isOneInOneHour) {
			limit = (limit - begin) * 2;
		}

		for (var i = 0; i < limit; i++) {
			if (appointmentList != null && index < appointmentList.Count) {
				if (appointmentList [index].data.Hour == dt.Hour && appointmentList [index].data.Minute == dt.Minute) {
					cellList.Add (DayController.Instantiate (cellPrefabTransform, dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), appointmentList [index].description, false));
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