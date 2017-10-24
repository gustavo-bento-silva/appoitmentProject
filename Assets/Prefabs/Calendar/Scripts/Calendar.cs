using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class Calendar : MonoBehaviour {
	
	public int numberMaxOfMonths;

	void Start()
	{
		int month;
		int year;
		DateTime date = DateTime.Now;

		for(int i = 0; i < numberMaxOfMonths; i++)
		{
			month = (int)date.Month + i;
			year = date.Year;
			if(month + i > 12)
			{
				year++;
			}
//			page.GetComponent<CalendarPageController>().calendar[i].FillCalendar(month, year);
		}
	}
	
//	void PrintCalendar()
//	{
//		string line = "";
//		for (int i = 0; i < 6; i++)
//		{
//			for (int j = 0; j < 7; j++)
//			{
//				line = string.Concat(line, calendar[i][j].ToString() + " ");
//			}
//			Debug.Log (line);
//			line = "";
//		}
//	}

}
