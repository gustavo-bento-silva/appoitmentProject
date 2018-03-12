using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Delegates
{
	public delegate void GetUserMessages (List<MessageModel> messages);

	public delegate void GetResponsibleAppointments (List<AppointmentModel> appointments);

	public delegate void GetUserAppointments (List<AppointmentModel> appointments);

	public delegate void GetAllServicesProvided (List<ResponsibleModel> responsablesWithServices);

	public delegate void GetAllResponsibles (List<ResponsibleModel> responsables);

	public delegate void CreateNewAppointment (AppointmentModel appointment);

	public delegate void GetUserByID (UserModel user);

	public delegate void GetResponsiblesByID (ResponsibleModel responsable);

	public delegate void UserLoginSuccess ();

	public delegate void UserLoginFail (string error);

	public delegate void GeneralListenerSuccess ();

	public delegate void GeneralListenerFail (string error);

	public delegate void GetAllCompanies (List<CompanyModel> companies);


	
}
