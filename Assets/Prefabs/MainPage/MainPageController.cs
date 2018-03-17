using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class MainPageController : MonoBehaviour
{
	private enum MenuState
	{
		Opened,
		Closed
	}

	public static MainPageController _instance;

	public Text userName;
	public Canvas canvas;
	public GameObject menu;
	public GameObject background;
	public float time = 0f;
	public GameObject homeBadge;
	public GameObject messagesBadge;
	public GameObject newAppointmentBadge;
	public GameObject myAppointmentsBadge;
	MenuState menuState = MenuState.Closed;
	float xPosition;


	public static MainPageController GetMainPageINstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
		}
		if (DataManager.currentUser != null) {
			userName.text = DataManager.currentUser.name;
		}
	}

	public void OnHomeClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().DropAllPagesFromStack ();
		OnMenuClick ();
	}

	public void OnMessagesClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.MessagesPage);
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

	public void ActiveHomeBadge (int quantity)
	{
		var homeBadgeText = homeBadge.GetComponentInChildren<Text> ();
		homeBadgeText.text = quantity.ToString ();
		homeBadge.SetActive (true);
	}

	public void ActiveNewAppointmentBadge (int quantity)
	{
		var newAppointmentBadgeText = newAppointmentBadge.GetComponentInChildren<Text> ();
		newAppointmentBadgeText.text = quantity.ToString ();
		newAppointmentBadge.SetActive (true);
	}

	public void ActiveMessagesBadge (int quantity)
	{
		var messagesBadgeText = messagesBadge.GetComponentInChildren<Text> ();
		messagesBadgeText.text = quantity.ToString ();
		messagesBadge.SetActive (true);
	}

	public void ActiveMyAppointmentsBadge (int quantity)
	{
		var myAppointmentsBadgeText = myAppointmentsBadge.GetComponentInChildren<Text> ();
		myAppointmentsBadgeText.text = quantity.ToString ();
		myAppointmentsBadge.SetActive (true);
	}

	public void HideHomeBadge ()
	{
		homeBadge.SetActive (false);
	}

	public void HideNewAppointmentBadge ()
	{
		newAppointmentBadge.SetActive (false);
	}

	public void HideMessagesBadge ()
	{
		messagesBadge.SetActive (false);
	}

	public void HideMyAppointmentsBadge ()
	{
		myAppointmentsBadge.SetActive (false);
	}
}