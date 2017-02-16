using UnityEngine;
using System.Collections;

public class CreateCalendarAsChild : MonoBehaviour {

	public GameObject calendarPrefab;
	public CalendarPageController controller;

	public void FillList()
	{
		GameObject go = GameObject.Instantiate(calendarPrefab);
		go.transform.SetParent(gameObject.transform, false);
		controller.calendar[controller.index] = go.GetComponent<CalendarController>();
	}
}
