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
		GameObject goPrefab = PageNavFrameWork.PageNav.GetPageNavInstance().GetPagePrefabByEnum(PageNavFrameWork.PagesEnum.CalendarPage);
		GameObject page = GameObject.Instantiate(goPrefab);
		page.GetComponent<CalendarPageController>().FillList();

		for(int i = 0; i < numberMaxOfMonths; i++)
		{
			month = (int)date.Month + i;
			year = date.Year;
			if(month + i > 12)
			{
				year++;
			}
			page.GetComponent<CalendarPageController>().calendar[i].FillCalendar(month, year);
		}
		PageNavFrameWork.PageNav.GetPageNavInstance().PushPageToStack(page);
		page.GetComponentInChildren<CarouselControl>().Initialize();
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
