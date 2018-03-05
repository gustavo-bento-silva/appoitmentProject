using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Delegates
{

	public delegate void GetAllResponsibles (List<ResponsableModel> responsables);

	public delegate void GetUserByID (UserModel user);

	public delegate void GetResponsiblesByID (ResponsableModel responsable);

	public delegate void UserLoginSuccess ();

	public delegate void UserLoginFail (string error);

	public delegate void GeneralListenerSuccess ();

	public delegate void GeneralListenerFail (string error);


	
}
