using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using PageNavFrameWork;
using UnityEngine.UI;

public class CalendarPageController : PageController
{

	public Dropdown dropdown;

	void Start () {
		GetEmployerList();
	}
	
	void Update () {

	}

	void GetEmployerList()
	{
		List<String> namesList = new List<string>();
		dropdown.ClearOptions();
		foreach (var employee in DataMockedManager.employers)
		{
			namesList.Add(employee.name);
		}
		
		dropdown.AddOptions(namesList);
		dropdown.onValueChanged.AddListener(GetEmployeeSelected);

	}

	void GetEmployeeSelected(int newPosition)
	{
		Debug.Log(newPosition);
	}
		
}
