using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Delegates
{
	public delegate void GetUserMessages (List<MessageModel> messages);

	public delegate void GetResponsibleAppointments (List<AppointmentModel> appointments);

	public delegate void GetUserAppointments (List<AppointmentModel> appointments);

	public delegate void GetAllServicesProvided (List<ResponsibleModel> responsablesWithServices);

	public delegate void GetAllServicesProvidedFromCompany (List<ServicesProvidedModel> services);

	public delegate void GetAllResponsibles (List<ResponsibleModel> responsables);

	public delegate void GetAllClients (List<UserModel> users);

	public delegate void GetDaysWorked (List<bool> daysWorked);

	public delegate void GetDaysTimeWorked (List<int> daysTime);

	public delegate void GetCompanyID (string companyID);

	public delegate void CreateNewAppointment (AppointmentModel appointment);

	public delegate void CreateNewUser (string userID);

	public delegate void GetUserByID (UserModel user);

	public delegate void GetResponsiblesByID (ResponsibleModel responsable);

	public delegate void UserLoginSuccess ();

	public delegate void UserLoginFail (string error);

	public delegate void GeneralListenerSuccess ();

	public delegate void GeneralListenerFail (string error);

	public delegate void GetAllCompanies (List<CompanyModel> companies);

	public delegate void OnSelectCompanyClick (CompanyModel company, int index);

	public delegate void OnSelectResponsibleClick (ResponsibleModel company, int index);

	public delegate void OnSelectServiceFromResponsibleClick (ServicesProvidedModel serviceprovided, int index);

	public delegate void OnSelectClientClick (UserModel user);

	public delegate void OnSelectServiceClick (ServicesProvidedModel serviceprovided, bool status);

	public delegate void OnSpriteSuccess (Sprite sprite);



}
