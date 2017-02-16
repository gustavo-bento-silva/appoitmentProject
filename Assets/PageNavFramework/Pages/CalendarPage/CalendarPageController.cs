using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;

public class CalendarPageController : PageController{

	public CalendarController[] calendar;
	public int index;
	public CreateCalendarAsChild[] child;

	void Awake () {
		calendar = new CalendarController[child.Length];
		for (index = 0; index < child.Length; index++)
		{
			child[index].FillList();
		}
	}
	
	void Update () {

	}
}
