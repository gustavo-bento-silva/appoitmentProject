using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GetButtonDay : MonoBehaviour {

	public void OnButtonSelected()
	{
		CalendarController cpc = gameObject.transform.parent.parent.GetComponent<CalendarController>();
		cpc.date = new System.DateTime(cpc.
		gameObject.GetComponentInChildren<Text>().text ;
	}
}
