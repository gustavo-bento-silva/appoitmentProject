using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;

public class CalendarPageController : PageController{

	public DateTime date;
	public CalendarController[] calendar;
	public int index;
	public CreateCalendarAsChild[] child;

	void Start () {

	}

	public void FillList()
	{
		calendar = new CalendarController[child.Length];
		for (index = 0; index < child.Length; index++)
		{
			child[index].FillList();
		}
	}
	
	void Update () {

	}

	public void OnLoadAppointmentPage()
	{
		GameObject goPrefab = PageNavFrameWork.PageNav.GetPageNavInstance().GetPagePrefabByEnum(PageNavFrameWork.PagesEnum.AppoitmentPage);
		GameObject page = GameObject.Instantiate(goPrefab);
		PageNavFrameWork.PageNav.GetPageNavInstance().PushPageToStack(page);
	}
}
