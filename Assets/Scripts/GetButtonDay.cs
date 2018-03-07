using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GetButtonDay : MonoBehaviour
{

	public Text dayText;

	public void Start ()
	{
		dayText = gameObject.GetComponentInChildren<Text> ();
	}

	public void OnButtonSelected ()
	{
		CalendarController cpc = gameObject.transform.parent.parent.GetComponent<CalendarController> ();
		int month = PlayerPreferences.TranslateMonth (cpc._month.text);
		int year = int.Parse (cpc._year.text);
		int day = int.Parse (dayText.gameObject.name);
	}
}
