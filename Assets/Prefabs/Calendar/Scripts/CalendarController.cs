﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour {

	public GameObject[] lines;
	public Text _month;
	public Text _year;
	public Color todayColor;

	Image[][] background;
	Text[][] linesText;
	// Use this for initialization
	void Start () {
		InitializeVariables ();
		FillCalendar (DateTime.Now.Month, DateTime.Now.Year);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitializeVariables()
	{
		linesText = new Text[lines.Length][];
		background = new Image[lines.Length][];
		for (int i = 0; i < lines.Length; i++)
		{
			Text[] temp = lines[i].GetComponentsInChildren<Text>();
			linesText[i] = new Text[temp.Length];
			background[i] = new Image[temp.Length];
			linesText[i] = lines[i].GetComponentsInChildren<Text>();
			background[i] = lines[i].GetComponentsInChildren<Image>();
		}
	}

	string GetMonth(int month)
	{
		switch (month)
		{
			case 1:
				return "Janeiro";
			case 2:
				return "Fevereiro";
			case 3:
				return "Março";
			case 4:
				return "Abril";
			case 5:
				return "Maio";
			case 6:
				return "Junho";
			case 7:
				return "Julho";
			case 8:
				return "Agosto";
			case 9:
				return "Setembro";
			case 10:
				return "Outubro";
			case 11:
				return "Novembro";
			case 12:
				return "Dezembro";
		}

		return "";
	}

	public void FillCalendar(int month, int year)
	{
		DateTime today = DateTime.Now;
		_month.text = GetMonth(month);
		_year.text = year.ToString();
		DateTime date = new DateTime(year,month,1);
		int dayOfWeek = (int)date.DayOfWeek;
		int day = 1;
		int daysInMonth = System.DateTime.DaysInMonth(year, month);

		InitializeVariables();

		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				if((int)today.Month == month && (int)today.Year == year && (int)today.Day == day)
				{
					background[i][j].color = todayColor;
				}
				if(i == 0 && j < dayOfWeek || day > daysInMonth)
				{
				}
				else{
					linesText[i][j].text = day.ToString();
					linesText[i][j].name = day.ToString();
					day++;
				}
			}
		}
	}

	public static GameObject Instantiate(GameObject calendarPrefab, int month, int year)
	{
		GameObject prefab = GameObject.Instantiate(calendarPrefab);
		prefab.GetComponent<CalendarController>().FillCalendar(month, year);
		return prefab;
	}
}
