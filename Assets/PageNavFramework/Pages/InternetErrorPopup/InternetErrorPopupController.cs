using UnityEngine;
using System.Collections;
using PageNavFrameWork;

public class InternetErrorPopupController : PageController
{

	public void OnOKButtonClick ()
	{
		Constants.LoadHomePage ();
	}
}
