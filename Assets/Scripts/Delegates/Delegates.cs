using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Delegates
{
	public delegate void GetUserMessages(List<MessageModel> messages);

	public delegate void GetResponsibleAppointments(List<AppointmentModel> appointments);

	public delegate void GetResponsibleBlockDay(List<BlockDay> blockDayList);

	public delegate void GetUserAppointments(List<AppointmentModel> appointments);

	public delegate void GetAllServicesProvided(List<ResponsibleModel> responsablesWithServices);

	public delegate void GetAllServicesProvidedFromCompany(List<ServicesProvidedModel> services);

	public delegate void GetAllResponsibles(List<ResponsibleModel> responsables);

	public delegate void GetAllClients(List<UserModel> users);

	public delegate void GetDaysWorked(List<bool> daysWorked);

	public delegate void GetDaysTimeWorked(List<float> daysTime);

	public delegate void GetLunchTime(LunchTime lunchTime);

	public delegate void GetCompanyID(string companyID);

	public delegate void CreateNewAppointment(AppointmentModel appointment);

	public delegate void CreateNewUser(string userID);

	public delegate void GetUserByID(UserModel user);

	public delegate void GetResponsiblesByID(ResponsibleModel responsable);

	public delegate void UserLoginSuccess(string userId);

	public delegate void UserFacebookLoginSuccess(string userId, string userName);

	public delegate void UserLoginFail(string error);

	public delegate void IsThereUser(bool isThereUser);

	public delegate void GeneralListenerSuccess();

	public delegate void GeneralListenerFail(string error);

	public delegate void GetAllCompanies(List<CompanyModel> companies);

	public delegate void OnSelectCompanyClick(CompanyModel company, int index);

	public delegate void OnSelectResponsibleClick(ResponsibleModel company, int index);

	public delegate void OnSelectServiceFromResponsibleClick(ServicesProvidedModel serviceprovided, int index);

	public delegate void OnSelectClientClick(UserModel user);

	public delegate void OnSelectServiceClick(ServicesProvidedModel serviceprovided, bool status);

	public delegate void OnSpriteSuccess(Sprite sprite);

}
