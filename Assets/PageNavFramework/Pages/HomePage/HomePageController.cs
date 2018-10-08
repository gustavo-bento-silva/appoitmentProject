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

	void Start()
	{
		Loading = true;
		Debug.Log("MyTag: User's info is loading. User ID: " + DataManager.userID);
		DataManager.LoadUserInfoAux(DataManager.userID, delegate ()
		{
			Loading = false;
			var currentUserType = DataManager.currentUser.userType;
			if (currentUserType == Constants.UserType.User.ToString())
			{
				LoadClientHome();
			}
			else if (currentUserType == Constants.UserType.Admin.ToString())
			{
				LoadAdminHome();
			}
			else if (currentUserType == Constants.UserType.Company.ToString())
			{
				LoadCompanyHome();
			}
			else if (currentUserType == Constants.UserType.Responsible.ToString())
			{
				LoadResponsibleHome();
			}
			else if (currentUserType == Constants.UserType.Anonymous.ToString())
			{
				LoadAnonymousHome();
			}
		}, delegate (string error)
		{
			Loading = false;
			Constants.LoadHomePage();
		});

	}

	void LoadClientHome()
	{
		MainPageController.GetMainPageInstance().SetClientMenu();
		PageNavFrameWork.PageNav.GetPageNavInstance().PushPageToStack(PageNavFrameWork.PagesEnum.SelectCompany);
		newAppointmentButton.SetActive(true);
	}

	void LoadCompanyHome()
	{
		MainPageController.GetMainPageInstance().SetCompanyMenu();
		PageNavFrameWork.PageNav.GetPageNavInstance().PushPageToStack(PageNavFrameWork.PagesEnum.SelectCompany);
		newAppointmentButton.SetActive(true);
	}

	void LoadAdminHome()
	{
		MainPageController.GetMainPageInstance().SetAdminMenu();
		newAppointmentButton.SetActive(true);
	}

	void LoadResponsibleHome()
	{
		MainPageController.GetMainPageInstance().SetResponsibleMenu();
		PageNavFrameWork.PageNav.GetPageNavInstance().PushPageToStack(PageNavFrameWork.PagesEnum.SelectCompany);
		newAppointmentButton.SetActive(true);
	}

	void LoadAnonymousHome()
	{
		MainPageController.GetMainPageInstance().SetAnonymousMenu();
		newAppointmentButton.SetActive(true);
	}
}
