using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GetButtonDay : MonoBehaviour {

	public CalendarPageController calendarPage;

	public void OnButtonSelected()
	{
		CalendarController cpc = gameObject.transform.parent.parent.GetComponent<CalendarController>();
		calendarPage.date = new System.DateTime(Int32.Parse(cpc._year.text), Int32.Parse(cpc._month.text), Int32.Parse(gameObject.GetComponentInChildren<Text>().text));
	}
}
