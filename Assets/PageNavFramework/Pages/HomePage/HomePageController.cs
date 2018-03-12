using UnityEngine;
using System.Collections;
using PageNavFrameWork;

public class HomePageController : PageController
{

	void Start ()
	{
		Loading = true;
		PageNavInstance.OpenModal (PageNavInstance.GetPagePrefabByEnum (PagesEnum.LoginPage));
	}

	void Update ()
	{

	}
}
