using UnityEngine;
using System.Collections;
using PageNavFrameWork;

public class LogoutPopupController : PageController
{
	public void Logout()
	{
		if (FirebaseAuth.GetFireBaseAuthInstance().UserLogout())
		{
			Constants.LoadLoginPage();
		}
	}
}
