using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PageNavFrameWork;
using UnityEngine.Networking.NetworkSystem;

public class ScheduleDetailPageController : PageController
{
	public RectTransform scrollViewContent;
	public Transform cellPrefabTransform;
	DateTime dt;

	private Vector3 OffSet = new Vector3 (165, -153, 0);
	private float contentOffset = 770;

	void Start ()
	{
		var dtNow = DateTime.Now;
		var minutes = 0;
		if (!PlayerPreferences.oneInOneHour) {
			minutes = 30;
		}

		dt = new DateTime (dtNow.Year, dtNow.Month, dtNow.Day, PlayerPreferences.initialTime, minutes, 0);
		InitializeScheduleTime (CreateApoointmentList ());
	}

	void Update ()
	{
	}

	List<AppointmentModel> CreateApoointmentList ()
	{
		var appointmentList = new List<AppointmentModel> ();
		appointmentList.Add (new AppointmentModel (new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, PlayerPreferences.initialTime, 30, 0),
			"teste", "Teste", "Ocupado"));
		return appointmentList;
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
		var limit = PlayerPreferences.endTime;
		var begin = PlayerPreferences.initialTime;
		var isOneInOneHour = PlayerPreferences.oneInOneHour;
		int index = 0;

		appointmentList.Sort ((first, second) => ((first.data).CompareTo (second.data)));

		if (!isOneInOneHour) {
			limit = (limit - begin) * 2;
		}

		for (var i = 0; i <= limit; i++) {
			if (appointmentList != null && index < appointmentList.Count) {
				if (appointmentList [index].data.Hour == dt.Hour && appointmentList [index].data.Minute == dt.Minute) {
					DayController.Instantiate (cellPrefabTransform, scrollViewContent,
						dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), appointmentList [index].description);
					index++;
				} else {
					DayController.Instantiate (cellPrefabTransform, scrollViewContent,
						dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre");
				}
			} else {
				DayController.Instantiate (cellPrefabTransform, scrollViewContent,
					dt.Hour.ToString () + ":" + dt.Minute.ToString ("00"), "Livre");
			}
			dt = dt.AddMinutes (isOneInOneHour ? 60 : 30);
		}

		ReadjustScrollSize (limit);
	}
}