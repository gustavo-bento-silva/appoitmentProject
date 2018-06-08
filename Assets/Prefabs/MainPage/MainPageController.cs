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

	public Color purpleColor;
	public Color greyColor;
	public Image menuHeader;
	public GameObject myAppointment;
	public GameObject manageResponsible;
	public GameObject manageServices;
	public GameObject manageClients;
	public GameObject companyAppointments;

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

	int messagesBadgesQuantity = 0;
	int appointmentsBadgesQuantity = 0;
	int homeBadgesQuantity = 0;

	public static MainPageController GetMainPageInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
		}
	}

	public void UpdateText ()
	{
		if (DataManager.currentUser != null) {
			userName.text = DataManager.currentUser.name;
		}
	}

	public void SetHeaderPurpleColor ()
	{
		menuHeader.color = purpleColor;
	}

	public void SetHeaderGrayColor ()
	{
		menuHeader.color = greyColor;
	}

	public void SetClientMenu ()
	{
		manageResponsible.SetActive (false);
		manageServices.SetActive (false);
		manageClients.SetActive (false);
		companyAppointments.SetActive (false);
	}

	public void SetCompanyMenu ()
	{
		myAppointment.SetActive (false);
	}

	public void SetResponsibleMenu ()
	{
		manageResponsible.SetActive (false);
		manageServices.SetActive (false);
		companyAppointments.SetActive (false);
	}

	public void OnHomeClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().DropAllPagesFromStack ();
		SetHeaderGrayColor ();
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

	public void OnServicesManagerClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.ManageServicesProvided);
		OnMenuClick ();
	}


	public void OnClientsManagerClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.ClientsManagerPage);
		OnMenuClick ();
	}

	public void OnResponsiblesManagerClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.ManageResponsiblePage);
		OnMenuClick ();
	}

	public void OnCompanyScheduleClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.CompanySchedulePage);
		OnMenuClick ();
	}

	public void OnContactClick ()
	{
		PageNavFrameWork.PageNav.GetPageNavInstance ().PushPageToStack (PageNavFrameWork.PagesEnum.ContactPage);
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
		iTween.MoveTo (menu, iTween.Hash ("x", xPosition, "time", time, "easeType", iTween.EaseType.linear.ToString ()));
	}

	void HideSideMenu ()
	{
		iTween.MoveTo (menu, iTween.Hash ("x", -xPosition, "time", time, "easeType", iTween.EaseType.linear.ToString ()));
	}

	public void ActiveHomeBadge ()
	{
		UpdateHomeBadgeText ();
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
		messagesBadgesQuantity += quantity;
		homeBadgesQuantity += quantity;
		ActiveHomeBadge ();
		var messagesBadgeText = messagesBadge.GetComponentInChildren<Text> ();
		messagesBadgeText.text = messagesBadgesQuantity.ToString ();
		messagesBadge.SetActive (true);
	}

	public void ActiveMyAppointmentsBadge (int quantity)
	{
		appointmentsBadgesQuantity += quantity;
		homeBadgesQuantity += quantity;
		ActiveHomeBadge ();
		var myAppointmentsBadgeText = myAppointmentsBadge.GetComponentInChildren<Text> ();
		myAppointmentsBadgeText.text = appointmentsBadgesQuantity.ToString ();
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
		CheckIfShouldHideHomeBadge (messagesBadgesQuantity);
		messagesBadgesQuantity = 0;
		messagesBadge.SetActive (false);
	}

	public void HideMyAppointmentsBadge ()
	{
		CheckIfShouldHideHomeBadge (appointmentsBadgesQuantity);
		appointmentsBadgesQuantity = 0;
		myAppointmentsBadge.SetActive (false);
	}

	public void CheckIfShouldHideHomeBadge (int quantity)
	{
		homeBadgesQuantity = homeBadgesQuantity - quantity;
		if (homeBadgesQuantity <= 0) {
			HideHomeBadge ();
		} else {
			UpdateHomeBadgeText ();
		}
	}

	void UpdateHomeBadgeText ()
	{
		var homeBadgeText = homeBadge.GetComponentInChildren<Text> ();
		homeBadgeText.text = homeBadgesQuantity.ToString ();
	}
}