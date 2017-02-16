using UnityEngine;
using System.Collections;

public class CreateCalendarAsChild : MonoBehaviour {

	public GameObject calendarPrefab;
	public CalendarPageController controller;

	public void FillList()
	{
		Object obj = GameObject.Instantiate((Object)calendarPrefab, this.gameObject.transform);
		GameObject go = obj as GameObject;
		controller.calendar[controller.index] = go.GetComponent<CalendarController>();
	}
}
