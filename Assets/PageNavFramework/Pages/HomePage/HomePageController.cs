using UnityEngine;
using System.Collections;
using PageNavFrameWork;

public class HomePageController : PageController
{
	public GameObject newAppointmentButton;
	public GameObject myAppointments;
	public GameObject myMessages;
	public GameObject userManager;
	public GameObject promotions;

	void Start ()
	{
		Loading = true;
		DataManager.LoadUserInfoAux (DataManager.userID, delegate() {
			Loading = false;
			var currentUserType = DataManager.currentUser.userType;
			if (currentUserType == Constants.UserType.User.ToString ()) {
				LoadClientHome ();
			} else if (currentUserType == Constants.UserType.Admin.ToString ()) {
				LoadAdminHome ();
			} else if (currentUserType == Constants.UserType.Company.ToString ()) {
				LoadCompanyHome ();
			} else if (currentUserType == Constants.UserType.Responsible.ToString ()) {
				LoadResponsibleHome ();
			}
		}, delegate(string error) {
			Loading = false;
			Constants.LoadHomePage ();
		});

	}

	void LoadClientHome ()
	{
		MainPageController.GetMainPageInstance ().SetClientMenu ();
		newAppointmentButton.SetActive (true);
		myAppointments.SetActive (true);
		myMessages.SetActive (true);
		promotions.SetActive (true);
	}

	void LoadCompanyHome ()
	{
		MainPageController.GetMainPageInstance ().SetCompanyMenu ();
		newAppointmentButton.SetActive (true);
		myAppointments.SetActive (true);
		myMessages.SetActive (true);
		userManager.SetActive (true);
	}

	void LoadAdminHome ()
	{
		newAppointmentButton.SetActive (true);
		myAppointments.SetActive (true);
		myMessages.SetActive (true);
		userManager.SetActive (true);
		promotions.SetActive (true);
	}

	void LoadResponsibleHome ()
	{
		MainPageController.GetMainPageInstance ().SetResponsibleMenu ();
		newAppointmentButton.SetActive (true);
		myAppointments.SetActive (true);
		myMessages.SetActive (true);
	}
}
