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
	Appoitments,
	User,
	Responsible,
	Company,
	Appointment,
	Data
}

public enum Parameters
{
	appointments,
	date,
	responsibles,
	servicesProvided
}

public class FireBaseManager : MonoBehaviour
{
	public static FireBaseManager _instance;

	string myProjectURL = "https://appointmentproject-a7233.firebaseio.com/";
	DatabaseReference reference;

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

	public void CreateNewAppoitment (UserModel user, ResponsibleModel responsable, AppointmentModel appointment)
	{
		string appoitmentID = reference.Child (DBTable.Appoitments.ToString ()).Push ().Key;
		appointment.AppointmentID = appoitmentID;

//		TODO Get appointments dinamicaly
//		user.appoitments[appoitmentID] = (object)responsable.userID;
//		responsable.appoitments[appoitmentID] = (object)user.userID;

		string json = JsonUtility.ToJson (appointment);

		CreateTable (DBTable.Appoitments, appoitmentID, json);
		reference.Child (DBTable.Appoitments.ToString ()).Child (appoitmentID).Child (Parameters.date.ToString ()).SetValueAsync (appointment.data.ToString (Constants.dateformat));
		reference.Child (DBTable.User.ToString ()).Child (user.userID).Child (Parameters.appointments + "/" + appoitmentID).SetRawJsonValueAsync (json);
		reference.Child (DBTable.Responsible.ToString ()).Child (responsable.userID).Child (Parameters.appointments + "/" + appoitmentID).SetRawJsonValueAsync (json);
	}

	public CompanyModel CreateNewCompany (string name, string phone, string city, string address, string cep)
	{
		string companyID = reference.Child (DBTable.Company.ToString ()).Push ().Key;
		CompanyModel company = new CompanyModel (new UserModel (companyID, name, phone), city, address, cep);

		string json = JsonUtility.ToJson (company);
		CreateTable (DBTable.Company, companyID, json);
		return company;
	}

	public void AddServicesToResponsible (string companyID, ResponsibleModel responsible, List<ServicesProvidedModel> services)
	{
		foreach (var service in services) {
			service.serviceID = reference.Child (DBTable.Company.ToString ()).Push ().Key;
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

	public UserModel CreateNewUser (/*string userID,*/ string name, string phone)
	{
		string userID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		UserModel user = new UserModel (userID, name, phone);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	public ResponsibleModel CreateNewResponsibleToCompany (string companyID, string name, List<ServicesProvidedModel> servicesProvided, string phone = "")
	{
		string responsibleID = reference.Child (DBTable.Responsible.ToString ()).Push ().Key;
		ResponsibleModel responsable = new ResponsibleModel (new UserModel (responsibleID, name, phone));
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

	void GetDaySchedule (string date)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Appoitments.ToString ()).Child ("data").EqualTo (date)
			.ValueChanged += HandleValueChanged;

	}

	public void GetUserByID (string userID, Delegates.GetUserByID getUserById)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.User.ToString ()).Child (userID)
			.GetValueAsync ().ContinueWith (task => {
			if (task.IsFaulted) {
				// Handle the error...
			} else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				var userJson = snapshot.GetRawJsonValue ();
				var user = JsonUtility.FromJson<UserModel> (userJson);
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

	void HandleValueChanged (object sender, ValueChangedEventArgs args)
	{
		if (args.DatabaseError != null) {
			Debug.LogError (args.DatabaseError.Message);
			return;
		}
//		Debug.Log ("!!!!" + args.Snapshot.);
	}
}
