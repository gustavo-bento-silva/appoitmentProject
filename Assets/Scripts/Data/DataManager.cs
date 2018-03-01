using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{

	public static List<AppointmentModel> appointmentList;
	public static CompanyModel companyData;
	public static List<ResponsableModel> employers = new List<ResponsableModel>();

	void Awake()
	{
	}

	void Start()
	{
//		GetUserByID();
//		CreateCompanyData();
	}

	List<AppointmentModel> CreateApoointmentList()
	{
		var appointmentList = new List<AppointmentModel>();
		appointmentList.Add(new AppointmentModel(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, PlayerPreferences.initialTime, 30, 0),
			"teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	CompanyModel CreateCompanyData()
	{
		companyData = FireBaseManager.GetFireBaseInstance().CreateNewCompany("Minha Empresa");
		
		var servicesList = new List<ServicesProvidedModel>();
		servicesList.Add(new ServicesProvidedModel("Cabeleireiro", 1));
		servicesList.Add(new ServicesProvidedModel("Manicure", 0.5f));
		servicesList.Add(new ServicesProvidedModel("Pedicure", 1.5f));
		FireBaseManager.GetFireBaseInstance().AddServicesToCompany(companyData, servicesList);
		
		employers.Add(FireBaseManager.GetFireBaseInstance().CreateNewResponsableToCompany(companyData.companyID, "Funcionario 1", new List<ServicesProvidedModel> {servicesList[0]}));
		employers.Add(FireBaseManager.GetFireBaseInstance().CreateNewResponsableToCompany(companyData.companyID, "Funcionario 2", new List<ServicesProvidedModel> {servicesList[1]}));
		employers.Add(FireBaseManager.GetFireBaseInstance().CreateNewResponsableToCompany(companyData.companyID, "Funcionario 3", new List<ServicesProvidedModel> {servicesList[2]}));
		employers.Add(FireBaseManager.GetFireBaseInstance().CreateNewResponsableToCompany(companyData.companyID, "Funcionario 4", servicesList));

		companyData.employees = employers.ToDictionary(x => x.userID, x => (object) x);
		
		return companyData;
	}

	public void GetUserByID()
	{
		FireBaseManager.GetFireBaseInstance().GetUserByID("-L6OPA2H1L7PpNcRW7Bh", (user) =>
			{
				FireBaseManager.GetFireBaseInstance().GetResponsibleByID("-L6MAdXb1xHw2o3kWYCx", (responsible) =>
					{
						CreateNewAppointment(user, responsible);
					});
			});
	}

	public static void CreateNewAppointment(UserModel user, ResponsableModel responsible)
	{
		var appointment = new AppointmentModel(DateTime.Now, user.userID, responsible.userID);
		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(user, responsible, appointment);
	}

	public static void GetAllResponsables(Delegates.GetAllResponsibles getAllResponsiblesListener)
	{
		getAllResponsiblesListener += responsibles => employers = responsibles;
		FireBaseManager.GetFireBaseInstance().GetAllResponsiblesFromCompany("-L6MAdWzOuaopL3vJqN4", getAllResponsiblesListener);
	}

//	void CreateAppointments()
//	{
//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("1", "Gustavo"), new ResponsableModel("1", "Bento"));
//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("2", "Thamyris"), new ResponsableModel("2", "Galvão"));
//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("3", "Marcia"), new ResponsableModel("3", "Perli"));
//	}
}
