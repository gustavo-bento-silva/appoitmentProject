using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;
using System;

public class AppoitmentPageController : PageController{

	public DateTime date;
	public GameObject cellTimePrefab;
	public GameObject content;

	int cellHeigth = 172;
	int cellSpacing = 5;

	void Start () {

		int quantity = PlayerPreferences.endTime - PlayerPreferences.initialTime;
		float time = PlayerPreferences.initialTime;
		if(!PlayerPreferences.oneInOneHour)
		{
			quantity *= 2;
		}
		for (int i = 0; i < quantity; i++)
		{
			GameObject go = GameObject.Instantiate(cellTimePrefab);
			if(PlayerPreferences.oneInOneHour){
				go.GetComponent<DayController>().time.text = time.ToString() + ":00";
				time++;
			}
			else{
				if(time % 1 == 0)
				{
					go.GetComponent<DayController>().time.text = time.ToString() + ":00";
				}
				else{
					go.GetComponent<DayController>().time.text = Mathf.Floor(time).ToString() + ":30"; 
				}
				time = time + 0.5f;
			}
			go.transform.SetParent(content.transform, false);

			var borderHeigth = (cellSpacing*(quantity-1))+cellHeigth*quantity-(content.transform as RectTransform).rect.height;
			if(borderHeigth > 0){
				(content.transform as RectTransform).sizeDelta = new Vector2 (0,borderHeigth);
			}
		}
	}
	
	void SaveAppointment()
	{

	}
}
