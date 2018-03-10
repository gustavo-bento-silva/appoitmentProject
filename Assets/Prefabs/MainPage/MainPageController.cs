using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class MainPageController : MonoBehaviour
{
	private enum MenuState
	{
		Opened,
		Closed
	}

	public Canvas canvas;
	public GameObject menu;
	public GameObject background;
	public float time = 0f;
	MenuState menuState = MenuState.Closed;
	float xPosition;

	// Use this for initialization
	void Start ()
	{
        
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public void OnHomeClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().DropAllPagesFromStack ();
		OnMenuClick ();
	}

	public void OnNewAppointmentClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.SelectCompany);
		OnMenuClick ();
	}

	public void OnMyAppointmentsClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.MyAppointmentsPage);
		OnMenuClick ();
	}

	public void OnMenuClick ()
	{
		if (menuState == MenuState.Closed) {
			menuState = MenuState.Opened;
			background.SetActive (true);
			ShowSideMenu ();
		} else {
			menuState = MenuState.Closed;
			background.SetActive (false);
			HideSideMenu ();
		}
	}

	void ShowSideMenu ()
	{
		xPosition = (((RectTransform)menu.transform).sizeDelta.x * canvas.scaleFactor) / 2;
		Debug.Log (xPosition);
		iTween.MoveTo (menu, iTween.Hash ("x", xPosition, "time", time, "easeType", iTween.EaseType.linear.ToString ()));
	}

	void HideSideMenu ()
	{
		iTween.MoveTo (menu, iTween.Hash ("x", -xPosition, "time", time, "easeType", iTween.EaseType.linear.ToString ()));
	}
}