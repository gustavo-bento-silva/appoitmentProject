using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	public static UserModel currentUser;
	public static List<MessageModel> userMessages = new List<MessageModel> ();
	public static List<AppointmentModel> appointmentList = new List<AppointmentModel> ();
	public static CompanyModel companyData;
	public static ResponsibleModel currentResponsible;
	public static ServicesProvidedModel currentservice;
	public static List<CompanyModel> companiesList = new List<CompanyModel> ();
	public static List<ResponsibleModel> responsibles = new List<ResponsibleModel> ();

	public static DateTime dateNewAppointment;

	void Awake ()
	{
	}

	void Start ()
	{
//		GetUserByID();
//		CreateCompanyData ();
		//		CreateCompanyDataTest2 ();
//		CreateUserJustForTest ();
		LogUser ("-L7FT39g_S2jGWgx4WOc");
	}

	//	List<AppointmentModel> CreateApoointmentList ()
	//	{
	//		var appointmentList = new List<AppointmentModel> ();
	//		appointmentList.Add (new AppointmentModel (new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, PlayerPreferences.initialTime, 30, 0),
	//			"teste", "Teste", "Ocupado"));
	//		return appointmentList;
	//	}

	void CreateUserJustForTest ()
	{
		FireBaseManager.GetFireBaseInstance ().CreateNewUser ("Gustavinho", "35442543");
	}

	void LogUser (string ID)
	{
		FireBaseManager.GetFireBaseInstance ().GetUserByID (ID, delegate(UserModel user) {
			currentUser = user;
			ActiveListeners ();
		});
	}

	void ActiveListeners ()
	{
		ActiveUserMessagesListener ();
		GetCurrentUserMessages (delegate() {
			
		}, delegate(string error) {
			
		});
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
			currentUser = user;
		});
	}

	public static void CreateNewAppointmentToCurrentUser (Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		var appointment = new AppointmentModel (dateNewAppointment, currentUser.userID, currentResponsible.userID, currentResponsible.name, currentservice.name);
		FireBaseManager.GetFireBaseInstance ().CreateNewAppoitment (currentUser, currentResponsible, appointment, delegate(AppointmentModel mappointment) {
			currentUser.appoitments.Add (mappointment.appointmentID, appointment);
			success ();
			string minute = (dateNewAppointment.Minute == 0 ? "00" : "0");
			var message = string.Format ("{0} realizou novo agendamento de {1} para dia {2}/{3} às {4}:{5}h", currentUser.name, currentservice.name, dateNewAppointment.Day, dateNewAppointment.Month, dateNewAppointment.Hour, minute);
			CreateNewMessageFromCurrentUserToResponsilbe (currentResponsible.userID, message, delegate() {
				
			}, delegate(string error) {
				
			});
		}, fail);
	}

	public static void CreateNewAppointment (UserModel user, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		var appointment = new AppointmentModel (dateNewAppointment, user.userID, currentResponsible.userID, currentResponsible.name, currentservice.name);
		FireBaseManager.GetFireBaseInstance ().CreateNewAppoitment (user, currentResponsible, appointment, delegate(AppointmentModel mappointment) {
			currentUser.appoitments.Add (mappointment.appointmentID, mappointment);
			success ();
		}, fail);
	}

	public static void CreateNewMessageFromCurrentUserToResponsilbe (string responsibleID, string message, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		var mMessage = new MessageModel (currentUser.name, message, DateTime.Now.ToString (Constants.dateformat));
		FireBaseManager.GetFireBaseInstance ().CreateNewMessage (mMessage, responsibleID, currentUser.userID, delegate() {
			success ();
		}, delegate(string error) {
			fail (error);
		});
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
			Debug.LogError ("Erro ao pegar os servicos: " + error);
		});
	}

	public static void GetResponsibleAppointments (Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().GetResponsibleAppointments (currentResponsible.userID, delegate(List<AppointmentModel> appointments) {
			appointmentList.Clear ();
			appointments.ForEach (x => appointmentList.Add (x));
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	public static void RemoveAppointmentFromUser (AppointmentModel appointment, Delegates.GeneralListenerSuccess success)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteAppointment (appointment, delegate() {
			currentUser.appoitments.Remove (appointment.appointmentID);

			string minute = (appointment.minute == "0" ? "00" : "0");
			var message = string.Format ("{0} desmarcou o agendamento de {1} marcado para dia {2} às {3}:{4}h", currentUser.name, appointment.description, appointment.data, appointment.hour, minute);
			CreateNewMessageFromCurrentUserToResponsilbe (currentResponsible.userID, message, delegate() {
				
			}, delegate(string error) {
				
			});
			success ();
		});
	}

	public static void RemoveMessage (MessageModel message, Delegates.GeneralListenerSuccess success)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteMessage (currentUser, message.id, delegate() {
			userMessages.Remove (message);
			success ();
		});
	}

	public static void ActiveUserMessagesListener ()
	{
		FireBaseManager.GetFireBaseInstance ().ActiveUserMessagesListener (currentUser.userID, delegate(List<MessageModel> messages) {
			userMessages.Clear ();
			currentUser.messages.Clear ();
			currentUser.messages = messages.ToDictionary (x => x.id, y => (object)y);
			userMessages = messages;
		});
	}

	public static void GetCurrentUserMessages (Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().GetUserMessages (currentUser.userID, delegate(List<MessageModel> messages) {
			userMessages.Clear ();
			userMessages = messages;
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	//	void CreateAppointments()
	//	{
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("1", "Gustavo"), new ResponsableModel("1", "Bento"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("2", "Thamyris"), new ResponsableModel("2", "Galvão"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("3", "Marcia"), new ResponsableModel("3", "Perli"));
	//	}
}
