using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;

public enum DBTable
{
	Appointments,
	User,
	Responsible,
	Company,
	Data
}

public enum Parameters
{
	appointments,
	messages,
	date,
	responsibles,
	clients,
	servicesProvided,
	isNew,
	daysOfWork,
	companyID,
	timeToBeginWork,
	timeToFinishWork
}

public class FireBaseManager : MonoBehaviour
{
	public static FireBaseManager _instance;

	string myProjectURL = "https://appointmentproject-a7233.firebaseio.com/";
	DatabaseReference reference;
	public Delegates.GetUserMessages userMessages;
	public Delegates.GetUserAppointments userAppointments;

	public static FireBaseManager GetFireBaseInstance ()
	{
		return _instance;
	}

	void Awake ()
	{
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (myProjectURL);
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		
		if (_instance == null) {
			_instance = this;
		}
	}

	void Start ()
	{
		
		/*UserModel user = CreateNewUser ("Gustavo");
		ResponsableModel responsable = CreateNewResponsable ("Geber");

		CreateNewAppoitment ("2017-10-24", user, responsable, "08:00");*/

	}

	public void CreateNewAppoitment (UserModel user, ResponsibleModel responsable, AppointmentModel appointment, Delegates.CreateNewAppointment success, Delegates.GeneralListenerFail fail)
	{
		string appoitmentID = reference.Child (DBTable.Appointments.ToString ()).Push ().Key;
		appointment.appointmentID = appoitmentID;

//		TODO Get appointments dinamicaly
//		user.appoitments[appoitmentID] = (object)responsable.userID;
//		responsable.appoitments[appoitmentID] = (object)user.userID;

		string json = JsonUtility.ToJson (appointment);

		CreateTable (DBTable.Appointments, appoitmentID, json);
		reference.Child (DBTable.User.ToString ()).Child (user.userID).Child (Parameters.appointments + "/" + appoitmentID).SetRawJsonValueAsync (json).ContinueWith (task2 => {
			if (task2.IsFaulted) {
				fail (task2.Exception.ToString ());
			} else if (task2.IsCompleted) {
				reference.Child (DBTable.Responsible.ToString ()).Child (responsable.userID).Child (Parameters.appointments + "/" + appoitmentID).SetRawJsonValueAsync (json).ContinueWith (task3 => {
					if (task3.IsFaulted) {
						fail (task3.Exception.ToString ());
					} else if (task3.IsCompleted) {
						success (appointment);
					}
				});
			}
		});
	}

	public void CreateNewMessage (MessageModel message, string responsibleID, string userID, string companyID, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		string messageID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		message.id = messageID;

		string json = JsonUtility.ToJson (message);

		reference.Child (DBTable.User.ToString ()).Child (userID).Child (Parameters.messages + "/" + messageID).SetRawJsonValueAsync (json).ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				reference.Child (DBTable.Responsible.ToString ()).Child (responsibleID).Child (Parameters.messages + "/" + messageID).SetRawJsonValueAsync (json).ContinueWith (task2 => {
					if (task2.IsFaulted) {
						fail (task2.Exception.ToString ());
					} else if (task2.IsCompleted) {
						reference.Child (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.messages + "/" + messageID).SetRawJsonValueAsync (json).ContinueWith (task3 => {
							if (task3.IsFaulted) {
								fail (task3.Exception.ToString ());
							} else if (task3.IsCompleted) {
								success ();
							}
						});
					}
				});
			}
		});
	}

	public void CreateNewMessageScheduleByCompany (MessageModel message, string responsibleID, string userID, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		string messageID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		message.id = messageID;

		string json = JsonUtility.ToJson (message);

		reference.Child (DBTable.Company.ToString ()).Child (userID).Child (Parameters.messages + "/" + messageID).SetRawJsonValueAsync (json).ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				reference.Child (DBTable.Responsible.ToString ()).Child (responsibleID).Child (Parameters.messages + "/" + messageID).SetRawJsonValueAsync (json).ContinueWith (task2 => {
					if (task2.IsFaulted) {
						fail (task2.Exception.ToString ());
					} else if (task2.IsCompleted) {
						success ();
					}
				});
			}
		});
	}

	public CompanyModel CreateNewCompany (string companyID, string name, string phone, string city, string address, string cep)
	{
		CompanyModel company = new CompanyModel (new UserModel (companyID, name, phone), city, address, cep);

		string json = JsonUtility.ToJson (company);
		CreateTable (DBTable.Company, companyID, json);
		CreateTable (DBTable.User, companyID, json);
		return company;
	}

	public void UpdateMyAppointment (UserModel user, string appointmentID)
	{
		if (user.userType == Constants.UserType.Responsible.ToString ()) {
			reference.Child (DBTable.Responsible.ToString ()).Child (user.userID).Child (Parameters.appointments.ToString ()).Child (appointmentID).Child (Parameters.isNew.ToString ()).SetValueAsync (false);
		} else {
			reference.Child (DBTable.User.ToString ()).Child (user.userID).Child (Parameters.appointments.ToString ()).Child (appointmentID).Child (Parameters.isNew.ToString ()).SetValueAsync (false);
		}
		reference.Child (DBTable.Appointments.ToString ()).Child (appointmentID).Child (Parameters.isNew.ToString ()).SetValueAsync (false);
	}

	public void UpdateMessage (UserModel user, MessageModel message)
	{
		if (user.userType == Constants.UserType.Responsible.ToString ()) {
			reference.Child (DBTable.Responsible.ToString ()).Child (user.userID).Child (Parameters.messages.ToString ()).Child (message.id).Child (Parameters.isNew.ToString ()).SetValueAsync (false);
		} else {
			reference.Child (DBTable.User.ToString ()).Child (user.userID).Child (Parameters.messages.ToString ()).Child (message.id).Child (Parameters.isNew.ToString ()).SetValueAsync (false);
		}
	}

	public void AddServicesToCompany (string companyID, List<ServicesProvidedModel> services, Delegates.GetAllServicesProvidedFromCompany success)
	{
		var servicesList = new List<ServicesProvidedModel> ();
		int index = 0;
		foreach (var service in services) {
			service.serviceID = reference.Child (DBTable.Company.ToString ()).Push ().Key;
			string json = JsonUtility.ToJson (service);
			reference.Child (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.servicesProvided.ToString ()).Child (service.serviceID).SetRawJsonValueAsync (json).ContinueWith (task => {
				if (task.IsCompleted) {
					index++;
					servicesList.Add (service);
					if (index >= services.Count) {
						success (servicesList);
					}
				}
			});
		}
	}

	public void AddServicesToResponsible (string companyID, ResponsibleModel responsible, List<ServicesProvidedModel> services)
	{
		foreach (var service in services) {
			responsible.servicesProvided.Add (service.serviceID, (object)service);
			string json = JsonUtility.ToJson (service);
			reference.Child (DBTable.Responsible.ToString ()).Child (responsible.userID).Child (Parameters.servicesProvided.ToString ()).Child (service.serviceID).SetRawJsonValueAsync (json);
			reference.Child (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.responsibles.ToString ()).Child (responsible.userID).Child (Parameters.servicesProvided.ToString ()).Child (service.serviceID).SetRawJsonValueAsync (json);
		}

	}

	public void AddEmployeeToCompany (string companyID, ResponsibleModel responsableModel, List<ServicesProvidedModel> servicesProvided)
	{
		string json = JsonUtility.ToJson (responsableModel);
		reference.Child (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.responsibles.ToString ()).Child (responsableModel.userID).SetRawJsonValueAsync (json).ContinueWith (task => {
			if (task.IsCompleted) {
				AddServicesToResponsible (companyID, responsableModel, servicesProvided);
			}
		});
	}

	public void AddClientToCompany (string companyID, UserModel userModel, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		string json = JsonUtility.ToJson (userModel);
		reference.Child (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.clients.ToString ()).Child (userModel.userID).SetRawJsonValueAsync (json).ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				success ();
			}
		});
	}

	public UserModel CreateNewClient (string name, string phone)
	{
		string userID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		UserModel user = new UserModel (userID, name, phone, Constants.UserType.Client);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	public UserModel CreateNewUser (string userID, string name, string phone)
	{
//		string userID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		UserModel user = new UserModel (userID, name, phone);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	public ResponsibleModel CreateNewResponsibleToCompany (string responsibleID, string companyID, string name, List<ServicesProvidedModel> servicesProvided, List<bool> daysWorked, List<int> initTime, List<int> finishTime, string phone)
	{
		ResponsibleModel responsable = new ResponsibleModel (new UserModel (responsibleID, name, phone), companyID, servicesProvided, daysWorked, initTime, finishTime);
		string json = JsonUtility.ToJson (responsable);

		CreateTable (DBTable.Responsible, responsibleID, json);
		CreateTable (DBTable.User, responsibleID, json);
		AddEmployeeToCompany (companyID, responsable, servicesProvided);
		return responsable;
	}

	public void CreateTable (DBTable table, string tableID, string json)
	{
		reference.Child (table.ToString ()).Child (tableID).SetRawJsonValueAsync (json);
	}

	public void GetAllResponsiblesFromCompany (String companyID, Delegates.GetAllResponsibles getAllResponsiblesListener)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.responsibles.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<ResponsibleModel> responsibles = new List<ResponsibleModel> ();
				foreach (var responsible in snapshot.Children) {
					string json = responsible.GetRawJsonValue ();
					responsibles.Add (JsonUtility.FromJson<ResponsibleModel> (json));
				}

				getAllResponsiblesListener (responsibles);
			}
		});
	}

	public void GetAllClientsFromCompany (String companyID, Delegates.GetAllClients getAllClientsListener, Delegates.GeneralListenerFail fail)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.clients.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<UserModel> clients = new List<UserModel> ();
				foreach (var client in snapshot.Children) {
					string json = client.GetRawJsonValue ();
					clients.Add (JsonUtility.FromJson<UserModel> (json));
				}

				getAllClientsListener (clients);
			}
		});
	}

	public void GetAllServicesProvidedFromCompany (String companyID, Delegates.GetAllServicesProvidedFromCompany servicesCallback)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.servicesProvided.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<ServicesProvidedModel> services = new List<ServicesProvidedModel> ();
				foreach (var service in snapshot.Children) {
					string json = service.GetRawJsonValue ();
					services.Add (JsonUtility.FromJson<ServicesProvidedModel> (json));
				}

				servicesCallback (services);
			}
		});
	}

	public void GetAllDaysWorkedFromCompany (String companyID, Delegates.GetDaysWorked daysWorkedCallback)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.daysOfWork.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<bool> daysWorkedList = new List<bool> ();
				foreach (var dayWorked in snapshot.Children) {
					daysWorkedList.Add ((bool)dayWorked.Value);
				}
				daysWorkedCallback (daysWorkedList);
			}
		});
	}

	public void GetAllInitDaysWorkedFromCompany (String companyID, Delegates.GetDaysTimeWorked daysTimeWorkedCallBack)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.timeToBeginWork.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<int> daysTime = new List<int> ();
				foreach (var daytime in snapshot.Children) {
					daysTime.Add ((int)daytime.Value);
				}
				daysTimeWorkedCallBack (daysTime);
			}
		});
	}

	public void GetAllFinishDaysWorkedFromCompany (String companyID, Delegates.GetDaysTimeWorked daysTimeWorkedCallBack)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.timeToFinishWork.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<int> daysTime = new List<int> ();
				foreach (var daytime in snapshot.Children) {
					daysTime.Add ((int)daytime.Value);
				}
				daysTimeWorkedCallBack (daysTime);
			}
		});
	}

	public void GetCompanyIDFromResponsible (String responsibleID, Delegates.GetCompanyID companyIDCallBack)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (responsibleID).Child (Parameters.companyID.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				companyIDCallBack ((string)snapshot.Value);
			}
		});
	}

	void GetDaySchedule (string date)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Appointments.ToString ()).Child ("data").EqualTo (date)
			.ValueChanged += HandleValueChanged;

	}

	public void GetUserByID (string userID, Delegates.GetUserByID getUserById)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID)
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				Debug.Log ("Falha no get user by id");
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				var userJson = snapshot.GetRawJsonValue ();
				var user = JsonUtility.FromJson<UserModel> (userJson);
//				GetUserAppointments (user.userID, delegate(List<AppointmentModel> appointments) {
//					user.appoitments = new Dictionary<string, object> ();
//					foreach (var appointment in appointments) {
//						user.appoitments.Add (appointment.appointmentID, (object)appointment);
//					}
//					getUserById (user);
//				}, delegate(string error) {
//					Debug.Log ("Falha no get user by id");
//				});

				getUserById (user);
			}
		});
	}

	public void GetResponsibleByID (string responsibleID, Delegates.GetResponsiblesByID getResponsiblesById)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (responsibleID)
		.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				var userJson = snapshot.GetRawJsonValue ();
				var responsible = JsonUtility.FromJson<ResponsibleModel> (userJson);
				getResponsiblesById (responsible);
					
			}
		});
		
	}

	public void GetAllCompanies (Delegates.GetAllCompanies getAllCompanies, Delegates.GeneralListenerFail getAllCompaniesFail)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				getAllCompaniesFail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<CompanyModel> companies = new List<CompanyModel> ();
				foreach (var company in snapshot.Children) {
					string json = company.GetRawJsonValue ();
					CompanyModel mcompany = JsonUtility.FromJson<CompanyModel> (json);
					companies.Add (mcompany);
				}
				getAllCompanies (companies);
			}
		});

	}

	public void UpdateServicesFromAllResponsibles (List <ResponsibleModel> responsibles, Delegates.GetAllServicesProvided success, Delegates.GeneralListenerFail fail)
	{
		int count = 0;
		List<ResponsibleModel> responsiblesWithServices = responsibles;
		foreach (var responsible in responsibles) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (responsible.userID).Child (Parameters.servicesProvided.ToString ())
				.GetValueAsync ().ContinueWith (task => {
				if (task.IsFaulted) {
					fail (task.Exception.ToString ());
				} else if (task.IsCompleted) {
					DataSnapshot snapshot = task.Result;
					List<ServicesProvidedModel> services = new List<ServicesProvidedModel> ();
					foreach (var service in snapshot.Children) {
						string json = service.GetRawJsonValue ();
						ServicesProvidedModel mservice = JsonUtility.FromJson<ServicesProvidedModel> (json);
						services.Add (mservice);
					}
					responsiblesWithServices [count].servicesProvided = services.ToDictionary (x => x.serviceID, x => (object)x);
					count++;
					if (count >= responsibles.Count) {
						success (responsiblesWithServices);
					}
				}
			});
		}
	}

	public void GetResponsibleAppointments (string responsibleID, Delegates.GetResponsibleAppointments success, Delegates.GeneralListenerFail fail)
	{

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (responsibleID).Child (Parameters.appointments.ToString ())
				.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<AppointmentModel> appointments = new List<AppointmentModel> ();
				foreach (var appointment in snapshot.Children) {
					string json = appointment.GetRawJsonValue ();
					AppointmentModel mAppointment = JsonUtility.FromJson<AppointmentModel> (json);
					appointments.Add (mAppointment);
				}
				success (appointments);
			}
		});
		
	}

	public void GetUserAppointments (string userID, Delegates.GetResponsibleAppointments success, Delegates.GeneralListenerFail fail)
	{

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).Child (Parameters.appointments.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<AppointmentModel> appointments = new List<AppointmentModel> ();
				foreach (var appointment in snapshot.Children) {
					string json = appointment.GetRawJsonValue ();
					AppointmentModel mAppointment = JsonUtility.FromJson<AppointmentModel> (json);
					appointments.Add (mAppointment);
				}
				success (appointments);
			}
		});

	}

	public void GetUserMessages (string userID, Delegates.GetUserMessages success, Delegates.GeneralListenerFail fail)
	{

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).Child (Parameters.messages.ToString ())
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				List<MessageModel> messages = new List<MessageModel> ();
				foreach (var message in snapshot.Children) {
					string json = message.GetRawJsonValue ();
					MessageModel mMessage = JsonUtility.FromJson<MessageModel> (json);
					messages.Add (mMessage);
				}
				success (messages);
			}
		});

	}

	public void RemoveUser (string userID, string userType, Delegates.GeneralListenerSuccess success)
	{
		if (userType == Constants.UserType.Responsible.ToString ()) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (userID).RemoveValueAsync ();
		}

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).RemoveValueAsync ()
			.ContinueWith (task => {
			if (task.IsFaulted) {

			} else {
				success ();
			}
		});

	}

	public void RemoveResponsibleFromCompany (string companyID, string userID, string userType, Delegates.GeneralListenerSuccess success)
	{
		if (userType == Constants.UserType.Responsible.ToString ()) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (userID).RemoveValueAsync ();
		}

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.responsibles.ToString ()).Child (userID).RemoveValueAsync ();

		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).RemoveValueAsync ()
			.ContinueWith (task => {
			if (task.IsFaulted) {

			} else {
				success ();
			}
		});

	}

	public void DeleteMessage (UserModel user, string messageID, Delegates.GeneralListenerSuccess success)
	{
		if (user is ResponsibleModel) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (user.userID).Child (Parameters.messages.ToString ()).Child (messageID).RemoveValueAsync ()
				.ContinueWith (task => {
				if (task.IsFaulted) {
						
				} else {
					success ();
				}
			});
		} else {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (user.userID).Child (Parameters.messages.ToString ()).Child (messageID).RemoveValueAsync ()
				.ContinueWith (task => {
				if (task.IsFaulted) {

				} else {
					success ();
				}
			});
			;
		}
	}

	public void DeleteService (String companyID, String serviceID)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (companyID).Child (Parameters.servicesProvided.ToString ()).Child (serviceID).RemoveValueAsync ();
	}

	public void DeleteAppointment (AppointmentModel appointment, Delegates.GeneralListenerSuccess success, Delegates.GeneralListenerFail fail)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Appointments.ToString ()).Child (appointment.appointmentID).RemoveValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				fail (task.Exception.ToString ());
			} else {
				FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (appointment.userID).Child (Parameters.appointments.ToString ()).RemoveValueAsync ().ContinueWith (task2 => {
					if (task2.IsFaulted) {
						fail (task2.Exception.ToString ());
					} else {
						FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (appointment.responsableID).Child (Parameters.appointments.ToString ()).RemoveValueAsync ().ContinueWith (task3 => {
							if (task3.IsFaulted) {
								fail (task3.Exception.ToString ());
							} else {
								success ();
							}
						});
					}
				});
			}
		});
	}

	public void ActiveUserMessagesListener (string userID, string userType, Delegates.GetUserMessages success)
	{
		userMessages += success;
		if (userType == Constants.UserType.Responsible.ToString ()) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (userID).Child (Parameters.messages.ToString ())
				.ValueChanged += MessagesListChanged;
		} else if (userType == Constants.UserType.Company.ToString ()) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child (userID).Child (Parameters.messages.ToString ())
				.ValueChanged += MessagesListChanged;
		} else {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).Child (Parameters.messages.ToString ())
				.ValueChanged += MessagesListChanged;
		}
	}

	public void ActiveMyAppointmentsListener (string userID, string userType, Delegates.GetUserAppointments appointmentsSuccess)
	{
		userAppointments += appointmentsSuccess;
		if (userType == Constants.UserType.Responsible.ToString ()) {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.Responsible.ToString ()).Child (userID).Child (Parameters.appointments.ToString ())
				.ValueChanged += AppointmentListChanged;
		} else {
			FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID).Child (Parameters.appointments.ToString ())
				.ValueChanged += AppointmentListChanged;
		}
	}

	void HandleValueChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
//		Debug.Log ("!!!!" + args.Snapshot.);
	}

	void AppointmentListChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
		DataSnapshot snapshot = args.Snapshot;
		List<AppointmentModel> appointments = new List<AppointmentModel> ();
		foreach (var appointment in snapshot.Children) {
			string json = appointment.GetRawJsonValue ();
			AppointmentModel mAppointment = JsonUtility.FromJson<AppointmentModel> (json);
			appointments.Add (mAppointment);
		}
		userAppointments (appointments);
	}

	void MessagesListChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
		List<MessageModel> messages = new List<MessageModel> ();
		foreach (var message in args.Snapshot.Children) {
			string json = message.GetRawJsonValue ();
			MessageModel mMessage = JsonUtility.FromJson<MessageModel> (json);
			messages.Add (mMessage);
		}
		userMessages (messages);
	}
}
