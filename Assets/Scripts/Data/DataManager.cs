using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	public static UserModel currentUser;
	public static List<MessageModel> userMessages = new List<MessageModel> ();
	public static List<AppointmentModel> responsibleAppointmentList = new List<AppointmentModel> ();
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
//		LoadUserInfo ("z0iJvJUBK2aK2BP2OAuACDrNMSn1");
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
		FireBaseManager.GetFireBaseInstance ().CreateNewUser ("123456", "Gustavinho", "35442543");
	}

	public static void LoadUserInfoAux (Delegates.GeneralListenerSuccess success)
	{
//		Empresa:
		string ID = "z0iJvJUBK2aK2BP2OAuACDrNMSn1";
//		Gustavo:
//		string ID = "4VpwAC7NBjVSW3ab86sgAnG1mC83";
		FireBaseManager.GetFireBaseInstance ().GetUserByID (ID, delegate(UserModel user) {
			if (user.userType == Constants.UserType.Company.ToString ()) {
				currentUser = new CompanyModel (user);
				GetAllResponsablesFromCompanyAsUser ();
				GetAllClientsFromCompanyAsUser ();
				GetAllServicesProvidedFromCompanyAsUser ();
				GetAllDaysWorkedFromCompanyAsUser ();
				GetAllInitWorkFromCompanyAsUser ();
				GetAllEndWorkFromCompanyAsUser ();
				companyData = new CompanyModel (user);
			} else if (user.userType == Constants.UserType.Responsible.ToString ()) {
				currentUser = new ResponsibleModel (user);
				GetCompanyIDFromResponsibleAsUser ();
				FireBaseManager.GetFireBaseInstance ().GetUserByID (ID, delegate(UserModel muser) {
					companyData = new CompanyModel (muser);
				});
			} else {
				currentUser = user;
			}
			FirebaseMessaging.SubscribeToTopic ();
			MainPageController.GetMainPageInstance ().UpdateText ();
			success ();
			ActiveListeners ();
		});
	}

	public static void LoadUserInfo (string ID)
	{
		FireBaseManager.GetFireBaseInstance ().GetUserByID (ID, delegate(UserModel user) {
			if (user.userType == Constants.UserType.Company.ToString ()) {
				currentUser = new CompanyModel (user);
				GetAllResponsablesFromCompanyAsUser ();
				GetAllClientsFromCompanyAsUser ();
				GetAllServicesProvidedFromCompanyAsUser ();
				GetAllDaysWorkedFromCompanyAsUser ();
				GetAllInitWorkFromCompanyAsUser ();
				GetAllEndWorkFromCompanyAsUser ();
				companyData = user as CompanyModel;
			} else if (user.userType == Constants.UserType.Responsible.ToString ()) {
				currentUser = user;
				FireBaseManager.GetFireBaseInstance ().GetUserByID (ID, delegate(UserModel muser) {
					companyData = muser as CompanyModel;
				});
			} else {
				currentUser = user;
			}
			ActiveListeners ();
		});
	}

	public static void GetUserById (string userID, Delegates.GetUserByID getUserSucess)
	{
		FireBaseManager.GetFireBaseInstance ().GetUserByID (userID, delegate(UserModel user) {
			getUserSucess (user);
		});
	}

	static void ActiveListeners ()
	{
		ActiveUserMessagesListener ();
		ActiveUserAppointmentsListener ();
//		GetCurrentUserMessages (delegate() {
//			
//		}, delegate(string error) {
//			
//		});
	}

	public static void CreateNewResponsibleToCompanyAsUser (string reponsibleID, string name, List<ServicesProvidedModel> services, List<bool> daysWorked, List<int> initTime, List<int> finishTime, string phone = "")
	{
		var mPhone = phone;
		if (string.IsNullOrEmpty (phone)) {
			mPhone = (currentUser as CompanyModel).phone;
		}
		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (reponsibleID, (currentUser as CompanyModel).userID, name, services, daysWorked, initTime, finishTime, mPhone));
	}

	public static void DeleteClient (UserModel client, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteClientToCompany (currentUser.userID, client, delegate() {
			(currentUser as CompanyModel).clients.Remove (client.userID);
			success ();
		}, fail);
	}

	public static void CreateNewClientToCompany (string name, string phone, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		var user = FireBaseManager.GetFireBaseInstance ().CreateNewClient (name, phone);
		FireBaseManager.GetFireBaseInstance ().AddClientToCompany (companyData.userID, user, delegate() {
			if (currentUser.userType == Constants.UserType.Company.ToString ()) {
				(currentUser as CompanyModel).clients.Add (user.userID, (object)user);
			}
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	public static void AddServiceToCompanyAsUser (ServicesProvidedModel service)
	{
		FireBaseManager.GetFireBaseInstance ().AddServicesToCompany (currentUser.userID, new List<ServicesProvidedModel> () { service }, delegate(List<ServicesProvidedModel> services) {
			services.ForEach (x => (currentUser as CompanyModel).servicesProvided.Add (x.serviceID, (object)x));
		});
	}

	public static void CreateCompanyDataWithMockData (string companyID)
	{
		companyData = FireBaseManager.GetFireBaseInstance ().CreateNewCompany (companyID, "Minha Empresa", "32456789", "Campinas", "Rua Joãozinho", "13082660");
		
//		var servicesList = new List<ServicesProvidedModel> ();
//		servicesList.Add (new ServicesProvidedModel ("Cabeleireiro", 1));
//		servicesList.Add (new ServicesProvidedModel ("Manicure", 0.5f));
//		servicesList.Add (new ServicesProvidedModel ("Pedicure", 1.5f));
		
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 1", new List<ServicesProvidedModel> { servicesList [0] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 2", new List<ServicesProvidedModel> { servicesList [1] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 3", new List<ServicesProvidedModel> { servicesList [2] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Funcionario 4", servicesList));

		companyData.employees = responsibles.ToDictionary (x => x.userID, x => (object)x);

	}

	CompanyModel CreateCompanyDataTest2 (string companyID)
	{
		companyData = FireBaseManager.GetFireBaseInstance ().CreateNewCompany (companyID, "Minha Empresa 2", "34565432", "Paulínia", "Rua Juarez Antonio Carlos", "13456765");

//		var servicesList = new List<ServicesProvidedModel> ();
//		servicesList.Add (new ServicesProvidedModel ("Cabeleireiro", 1));
//		servicesList.Add (new ServicesProvidedModel ("Pintura", 0.5f));
//		servicesList.Add (new ServicesProvidedModel ("Tinta", 1.5f));

//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 1", new List<ServicesProvidedModel> { servicesList [0] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 2", new List<ServicesProvidedModel> { servicesList [1] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 3", new List<ServicesProvidedModel> { servicesList [2] }));
//		responsibles.Add (FireBaseManager.GetFireBaseInstance ().CreateNewResponsibleToCompany (companyData.userID, "Meu Func 4", servicesList));

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
		CreateNewAppointment (currentUser, success, fail);
	}

	public static void CreateNewAppointment (UserModel user, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		var appointment = new AppointmentModel (dateNewAppointment, user.userID, user.name, currentResponsible.userID, currentResponsible.name, currentservice.name, Mathf.RoundToInt ((currentservice.duration) * 60));
		FireBaseManager.GetFireBaseInstance ().CreateNewAppoitment (user, currentResponsible, appointment, delegate(AppointmentModel mappointment) {
			success ();
			string minute = (dateNewAppointment.Minute == 0 ? "00" : "0");
			var message = string.Format ("{0} realizou novo agendamento de {1} para dia {2}/{3} às {4}:{5}h", user.name, currentservice.name, dateNewAppointment.Day, dateNewAppointment.Month, dateNewAppointment.Hour, minute);
			if (user.userType == Constants.UserType.User.ToString ()) {
				CreateNewMessageFromUserToResponsilbe (user, currentResponsible, message, Constants.MessageType.ScheduleAppointment.ToString (), delegate() {

				}, delegate(string error) {

				});
			} else {
				CreateNewMessageFromClientOrCompanyToResponsilbe (user, currentResponsible.userID, message, Constants.MessageType.ScheduleAppointment.ToString (), delegate() {

				}, delegate(string error) {

				});	
			}
		}, fail);
	}

	public static void CreateNewMessageFromUserToResponsilbe (UserModel from, ResponsibleModel responsible, string message, string type, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		string companyID = "";
		var mMessage = new MessageModel (from.name, responsible.userID, message, DateTime.Now.ToString (Constants.dateformat), type);
		if (from.userType == Constants.UserType.User.ToString ()) {
			FireBaseManager.GetFireBaseInstance ().CreateNewMessage (mMessage, from.userID, responsible.userID, responsible.companyID, delegate() {
				success ();
			}, delegate(string error) {
				fail (error);
			});
		} else {
			fail ("Usuário não é um profissional!");
		}

	}

	public static void CreateNewMessageFromClientOrCompanyToResponsilbe (UserModel from, string toID, string message, string type, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		string companyID = "";
		var mMessage = new MessageModel (from.name, toID, message, DateTime.Now.ToString (Constants.dateformat), type);
		if (from.userType == Constants.UserType.Responsible.ToString ()) {
			companyID = (from as ResponsibleModel).companyID;
			FireBaseManager.GetFireBaseInstance ().CreateNewMessage (mMessage, from.userID, toID, companyID, delegate() {
				success ();
			}, delegate(string error) {
				fail (error);
			});
		} else if (from.userType == Constants.UserType.Client.ToString ()) {
			FireBaseManager.GetFireBaseInstance ().CreateNewMessageScheduleByCompany (mMessage, toID, currentResponsible.companyID, delegate() {
				success ();
			}, delegate(string error) {
				fail (error);
			});
		} else {
			fail ("Usuário não é um profissional!");
		}

	}

	public static void UpdateService (ServicesProvidedModel service, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		bool mError = false;
		UpdateServiceCompany (service, delegate() {
			GetAllResponsablesFromCompany (companyData.userID, delegate(List<ResponsibleModel> responsables) {
				responsables.ForEach (x => {
					UpdateServiceResponsible (service, x.userID, delegate() {
						
					}, delegate(string error) {
						mError = true;
					});
				});
				if (mError) {
					fail ("");
				} else {
					success ();
				}
			});
		}, delegate(string error) {
			fail (error);
		});

	}

	public static void UpdateResponsibleServices (ResponsibleModel responsible, List<ServicesProvidedModel> services, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().RemoveAllServcesFromResponsible (responsible.companyID, responsible, delegate() {
			FireBaseManager.GetFireBaseInstance ().AddServicesToResponsible (responsible.companyID, responsible, services);
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	static void UpdateServiceCompany (ServicesProvidedModel service, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().UpdateSeviceCompany (companyData.userID, service, delegate() {
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	static void UpdateServiceResponsible (ServicesProvidedModel service, string responsibleID, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().UpdateSeviceResponsible (responsibleID, service, delegate() {
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	static void GetAllResponsablesFromCompany (string companyID, Delegates.GetAllResponsibles getResponsibles)
	{
		FireBaseManager.GetFireBaseInstance ().GetAllResponsiblesFromCompany (companyID, getResponsibles);
	}

	static void GetAllResponsablesFromCompanyAsUser ()
	{
		Delegates.GetAllResponsibles getAllResponsiblesListener = (mresponsibles) => {
			responsibles = mresponsibles;
			(currentUser as CompanyModel).employees = mresponsibles.ToDictionary (x => x.userID, x => (object)x);
		};
		FireBaseManager.GetFireBaseInstance ().GetAllResponsiblesFromCompany (currentUser.userID, getAllResponsiblesListener);
	}

	static void GetAllClientsFromCompanyAsUser ()
	{
		Delegates.GetAllClients getAllClients = (clients) => {
			(currentUser as CompanyModel).clients = clients.ToDictionary (x => x.userID, x => (object)x);
		};
		FireBaseManager.GetFireBaseInstance ().GetAllClientsFromCompany (currentUser.userID, getAllClients, delegate(string error) {
			
		});
	}

	public static void GetAllClientsFromCompany (string companyID, Delegates.GetAllClients getClientsSuccess)
	{
		FireBaseManager.GetFireBaseInstance ().GetAllClientsFromCompany (companyID, getClientsSuccess, delegate(string error) {

		});
	}


	static void GetAllServicesProvidedFromCompanyAsUser ()
	{
		Delegates.GetAllServicesProvidedFromCompany getAllServicesProvided = (services) => {
			(currentUser as CompanyModel).servicesProvided = services.ToDictionary (x => x.serviceID, x => (object)x);
		};
		FireBaseManager.GetFireBaseInstance ().GetAllServicesProvidedFromCompany (currentUser.userID, getAllServicesProvided);
	}


	static void GetAllDaysWorkedFromCompanyAsUser ()
	{
		Delegates.GetDaysWorked getDaysWorkedCallBack = delegate(List<bool> daysWorked) {
			(currentUser as CompanyModel).daysOfWork = daysWorked;
		};

		FireBaseManager.GetFireBaseInstance ().GetAllDaysWorkedFromCompany (currentUser.userID, getDaysWorkedCallBack);
	}

	static void GetAllInitWorkFromCompanyAsUser ()
	{
		Delegates.GetDaysTimeWorked getInitDaysWorkedCallBack = delegate(List<int> initTimeDays) {
			(currentUser as CompanyModel).timeToBeginWork = initTimeDays;
		};

		FireBaseManager.GetFireBaseInstance ().GetAllInitDaysWorkedFromCompany (currentUser.userID, getInitDaysWorkedCallBack);
	}

	static void GetAllEndWorkFromCompanyAsUser ()
	{
		Delegates.GetDaysTimeWorked getFinishDaysWorkedCallBack = delegate(List<int> finishTimeDays) {
			(currentUser as CompanyModel).timeToFinishWork = finishTimeDays;
		};

		FireBaseManager.GetFireBaseInstance ().GetAllFinishDaysWorkedFromCompany (currentUser.userID, getFinishDaysWorkedCallBack);
	}

	static void GetCompanyIDFromResponsibleAsUser ()
	{
		Delegates.GetCompanyID getCompanyIDCallBack = delegate(string companyID) {
			(currentUser as ResponsibleModel).companyID = companyID;
		};

		FireBaseManager.GetFireBaseInstance ().GetCompanyIDFromResponsible (currentUser.userID, getCompanyIDCallBack);
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
			responsibleAppointmentList.Clear ();
			appointments.ForEach (x => responsibleAppointmentList.Add (x));
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	public static void RemoveUser (UserModel user, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FirebaseAPIHelper.GetFireBaseAPIHelperInstance ().RemoveUser (user.userID, delegate() {
			FireBaseManager.GetFireBaseInstance ().RemoveUser (user.userID, user.userType, delegate() {
				GetAllResponsablesFromCompanyAsUser ();
				success ();
			});
		}, fail);
	}

	public static void RemoveResponsibleFromCompany (UserModel user, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FirebaseAPIHelper.GetFireBaseAPIHelperInstance ().RemoveUser (user.userID, delegate() {
			FireBaseManager.GetFireBaseInstance ().RemoveResponsibleFromCompany (currentUser.userID, user.userID, user.userType, delegate() {
				responsibles.Remove (user as ResponsibleModel);
				GetAllResponsablesFromCompanyAsUser ();
				success ();
			});
		}, fail);
	}

	public static void RemoveServiceFromCompany (String companyID, String serviceID)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteService (companyID, serviceID);
	}

	public static void RemoveServiceFromCompanyAsUser (String serviceID)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteService (currentUser.userID, serviceID);
	}

	public static void JustRemoveAppointmentWithouMessage (AppointmentModel appointment, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteAppointment (appointment, success, fail);
	}

	public static void RemoveAppointmentFromResponsible (AppointmentModel appointment, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteAppointment (appointment, delegate() {
			currentUser.appoitments.Remove (appointment.appointmentID);

			string minute = (appointment.minute.ToString () == "0" ? "00" : "0");
			var message = string.Format ("{0} desmarcou o agendamento de {1} marcado para dia {2} às {3}:{4}h", currentUser.name, appointment.description, appointment.data, appointment.hour, minute);
			string toID = "";
			if (currentUser.userType == Constants.UserType.Responsible.ToString ()) {
				toID = appointment.userID;
			} else {
				toID = appointment.responsableID;
			}
			CreateNewMessageFromClientOrCompanyToResponsilbe (currentUser, toID, message, Constants.MessageType.RemoveAppointment.ToString (), delegate() {

			}, delegate(string error) {

			});
			success ();
		}, delegate(string error) {
			fail (error);
		});
	}

	public static void RemoveAppointmentFromUser (AppointmentModel appointment, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FireBaseManager.GetFireBaseInstance ().DeleteAppointment (appointment, delegate() {
			currentUser.appoitments.Remove (appointment.appointmentID);

			string minute = (appointment.minute.ToString () == "0" ? "00" : "0");
			var message = string.Format ("{0} desmarcou o agendamento de {1} marcado para dia {2} às {3}:{4}h", currentUser.name, appointment.description, appointment.data, appointment.hour, minute);
			CreateNewMessageFromUserToResponsilbe (currentUser, currentResponsible, message, Constants.MessageType.RemoveAppointment.ToString (), delegate() {
				
			}, delegate(string error) {
			});
			success ();
		}, delegate(string error) {
			fail (error);
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
		FireBaseManager.GetFireBaseInstance ().ActiveUserMessagesListener (currentUser.userID, currentUser.userType, delegate(List<MessageModel> messages) {
			SetCurrentUserMessages (messages);
			userMessages = messages;
			DefineMessagesBadges ();
		});
	}

	public static void ActiveUserAppointmentsListener ()
	{
		FireBaseManager.GetFireBaseInstance ().ActiveMyAppointmentsListener (currentUser.userID, currentUser.userType, delegate(List<AppointmentModel> appointments) {
			SetCurrentUserAppointments (appointments);
			DefineAppointmentsBadges ();
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

	private static void DefineAppointmentsBadges ()
	{
		List<AppointmentModel> appointments = new List<AppointmentModel> ();
		foreach (var appointmentKey in currentUser.appoitments.Keys) {
			appointments.Add ((AppointmentModel)currentUser.appoitments [appointmentKey]);
		}
		if (appointments != null) {
			var i = 0;
			appointments.ForEach (x => {
				if (x.isNew)
					i++;
			});
			if (i > 0) {
				MainPageController.GetMainPageInstance ().ActiveMyAppointmentsBadge (i);
			} else {
				MainPageController.GetMainPageInstance ().HideMyAppointmentsBadge ();
			}
		}
	}

	private static void DefineMessagesBadges ()
	{
		if (userMessages != null) {
			var i = 0;
			userMessages.ForEach (x => {
				if (x.isNew)
					i++;
			});
			if (i > 0) {
				MainPageController.GetMainPageInstance ().ActiveMessagesBadge (i);
			} else {
				MainPageController.GetMainPageInstance ().HideMessagesBadge ();
			}
		}
	}

	public static void SetMyAppointmentsAsRead ()
	{
		if (currentUser.appoitments != null) {
			foreach (var appointmentKey in currentUser.appoitments.Keys) {
				AppointmentModel appointment;
				appointment = (AppointmentModel)currentUser.appoitments [appointmentKey];
				if (appointment.isNew) {
					appointment.isNew = false;
					FireBaseManager.GetFireBaseInstance ().UpdateMyAppointment (currentUser, appointmentKey);
				}
			}
		}
		DefineAppointmentsBadges ();
	}

	public static void SetMessagesToRead ()
	{
		userMessages.ForEach (x => {
			if (x.isNew) {
				x.isNew = false;
				FireBaseManager.GetFireBaseInstance ().UpdateMessage (currentUser, x);
			}
		});
		SetCurrentUserMessages (userMessages);
		DefineMessagesBadges ();

	}

	private static void SetCurrentUserMessages (List<MessageModel> messages)
	{
		if (currentUser.messages != null) {
			currentUser.messages.Clear ();	
		} else {
			currentUser.messages = new Dictionary<string, object> ();
		}
		currentUser.messages = messages.ToDictionary (x => x.id, y => (object)y);
	}

	private static void SetCurrentUserAppointments (List<AppointmentModel> appointments)
	{
		if (currentUser.appoitments != null) {
			currentUser.appoitments.Clear ();	
		} else {
			currentUser.appoitments = new Dictionary<string, object> ();
		}
		currentUser.appoitments = appointments.ToDictionary (x => x.appointmentID, x => (object)x);
	}

	//	void CreateAppointments()
	//	{
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("1", "Gustavo"), new ResponsableModel("1", "Bento"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("2", "Thamyris"), new ResponsableModel("2", "Galvão"));
	//		FireBaseManager.GetFireBaseInstance().CreateNewAppoitment(DateTime.Today, new UserModel("3", "Marcia"), new ResponsableModel("3", "Perli"));
	//	}
}
