using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class GetButtonDay : MonoBehaviour
{

	public Text dayText;

	public void Start ()
	{
		dayText = gameObject.GetComponentInChildren<Text> ();
		gameObject.GetComponent<Button> ().onClick.AddListener (OnButtonSelected);
	}

	public void OnButtonSelected ()
	{
		ConfigureDate ();
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.ScheduleDetailPage);
	}

	public void OnButtonSelectedWithArgs ()
	{
		ConfigureDate ();
		var dic = new Dictionary<string, object> ();
		dic.Add ("isFromScheduleAppointment", (object)true);
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PageNavFrameWork.PagesEnum.ScheduleDetailPage, dic);
	}

	void ConfigureDate ()
	{
		CalendarController cpc = gameObject.transform.parent.parent.GetComponent<CalendarController> ();
		int month = PlayerPreferences.TranslateMonth (cpc._month.text);
		int year = int.Parse (cpc._year.text);
		int day = int.Parse (dayText.gameObject.name);

		DataManager.dateNewAppointment = new DateTime (year, month, day);
	}


}
