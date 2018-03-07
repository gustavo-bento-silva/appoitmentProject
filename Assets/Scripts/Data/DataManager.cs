using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{

	public static List<AppointmentModel> appointmentList;
	public static CompanyModel companyData;
	public static ResponsibleModel currentResponsible;
	public static List<CompanyModel> companiesList = new List<CompanyModel> ();
	public static List<ResponsibleModel> responsibles = new List<ResponsibleModel> ();

	void Awake ()
	{
	}

	void Start ()
	{
//		GetUserByID();
//		CreateCompanyData ();
//		CreateCompanyDataTest2 ();
	}

	List<AppointmentModel> CreateApoointmentList ()
	{
		var appointmentList = new List<AppointmentModel> ();
		appointmentList.Add (new AppointmentModel (new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, PlayerPreferences.initialTime, 30, 0),
			"teste", "Teste", "Ocupado"));
		return appointmentList;
	}

	CompanyModel CreateCompanyData ()
	{
		companyData = FireBaseManager.GetFireBaseInstance ().CreateNewCompany ("Minha Empresa", "32456789", "Campinas", "Rua Joãozinho", "13082660");
		
		var servicesList = new List<ServicesProvidedModel> ();
		servicesList.Add (new ServicesProvidedModel ("Cabeleireiro", 1));
		servicesList.Add (new ServicesProvidedModel ("Manicure", 0.5f));
		servicesList.Add (new ServicesProvidedModel ("Pedicure", 1.5f));
		
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 1", new List<ServicesProvidedModel> { servicesList [0] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 2", new List<ServicesProvidedModel> { servicesList [1] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 3", new List<ServicesProvidedModel> { servicesList [2] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 4", servicesList));

		companyData.employees = responsibles.ToDictionary (x => x.userID, x => (object)x);
		
		return companyData;
	}

	CompanyModel CreateCompanyDataTest2 ()
	{
		companyData = FireBaseManager.GetFireBaseInstance ().CreateNewCompany ("Minha Empresa 2", "34565432", "Paulínia", "Rua Juarez Antonio Carlos", "13456765");

		var servicesList = new List<ServicesProvidedModel> ();
		servicesList.Add (new ServicesProvidedModel ("Cabeleireiro", 1));
		servicesList.Add (new ServicesProvidedModel ("Pintura", 0.5f));
		servicesList.Add (new ServicesProvidedModel ("Tinta", 1.5f));

		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 1", new List<ServicesProvidedModel> { servicesList [0] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 2", new List<ServicesProvidedModel> { servicesList [1] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 3", new List<ServicesProvidedModel> { servicesList [2] }));
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 4", servicesList));

		companyData.employees = responsibles.ToDictionary (x => x.userID, x => (object)x);

		return companyData;
	}

	public void GetUserByID ()
	{
		FireBaseManager.GetFireBaseInstance ().GetUserByID ("-L6OPA2H1L7PpNcRW7Bh", (user) => {
			FireBaseManager.GetFireBaseInstance ().GetResponsibleByID ("-L6MAdXb1xHw2o3kWYCx", (responsible) => {
				CreateNewAppointment (user, responsible);
			});
		});
	}

	public static void CreateNewAppointment (UserModel user, ResponsibleModel responsible)
	{
		var appointment = new AppointmentModel (DateTime.Now, user.userID, responsible.userID);
		FireBaseManager.GetFireBaseInstance ().CreateNewAppoitment (user, responsible, appointment);
	}

	public static void GetAllResponsablesFromCompany (Delegates.GetAllResponsibles getAllResponsiblesListener)
	{
		getAllResponsiblesListener += (mresponsibles) => responsibles = mresponsibles;
		FireBaseManager.GetFireBaseInstance ().GetAllResponsiblesFromCompany (companyData.userID, getAllResponsiblesListener);
	}

	public static void GetServicesFromAllResponsibles (Delegates.GetAllServicesProvided success)
	{
		success += (mresponsibles) => responsibles = mresponsibles;
		FireBaseManager.GetFireBaseInstance ().UpdateServicesFromAllResponsibles (responsibles, success, delegate(string error) {
			Debug.LogError ("Erro am pegar os servicos: " + error);
		});
	}

	//	void CreateAppointments()
	//	{
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("1", "Gustavo"), new ResponsableModel("1", "Bento"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("2", "Thamyris"), new ResponsableModel("2", "Galvão"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("3", "Marcia"), new ResponsableModel("3", "Perli"));
	//	}
}
