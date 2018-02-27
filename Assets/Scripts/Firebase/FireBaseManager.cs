using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;

public enum DBTable
{
	Appoitments,
	User,
	Responsable,
	Company,
	Appointment,
	Data
}

public enum Parameters
{
	appointments,
	date,
	responsables
}
	
public class FireBaseManager : MonoBehaviour
{
	public static FireBaseManager _instance;

	public static List<ResponsableModel> responsibles = new List<ResponsableModel>();
	public static List<ResponsableModel> userAppoitments = new List<ResponsableModel>();

	string myProjectURL = "https://appointmentproject-a7233.firebaseio.com/";
	DatabaseReference reference;
	Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	public static FireBaseManager GetFireBaseInstance(){
		return _instance;
	}

	void Awake (){
		
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (myProjectURL);
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		
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

	public void UserLogin(string email, string password){
		auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			user = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewUserWithEmailAndPassword(string email, string password)
	{
		auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				return;
			}

			// Firebase user has been created.
			user = task.Result;
			CreateNewUser(user.UserId, user.DisplayName);
			Debug.LogFormat("Firebase user created successfully: {0} ({1})",
				user.DisplayName, user.UserId);
		});
	}

	public void CreateNewAppoitment (DateTime data, UserModel user, ResponsableModel responsable, string description = " ", int duration = 30)
	{
		string appoitmentID = reference.Child (DBTable.Appoitments.ToString ()).Push ().Key;
		AppointmentModel appointment = new AppointmentModel (data, user.userID, responsable.responsableID, description, duration);

		user.appoitments[appoitmentID] = (object)responsable.responsableID;
//		responsable.appoitments[appoitmentID] = (object)user.userID;

		string json = JsonUtility.ToJson (appointment);

		CreateTable (DBTable.Appoitments, appoitmentID, json);
		reference.Child (DBTable.Appoitments.ToString ()).Child (appoitmentID).Child(Parameters.date.ToString()).SetValueAsync (data.ToString(Constants.dateformat));
		reference.Child (DBTable.User.ToString ()).Child (user.userID + "/appointments").SetValueAsync (user.appoitments);
//		reference.Child (DBTable.Responsable.ToString ()).Child (responsable.responsableID + "/appointments").SetValueAsync (responsable.appoitments);
	}

	public CompanyModel CreateNewCompany(string name)
	{
		string companyID = reference.Child (DBTable.Company.ToString ()).Push ().Key;
		CompanyModel company = new CompanyModel(companyID, name);

		string json = JsonUtility.ToJson (company);
		CreateTable(DBTable.Company, companyID, json);
		return company;
	}

	public void AddEmployeeToCompany(string companyID, ResponsableModel responsableModel)
	{
		string json = JsonUtility.ToJson (responsableModel);
		reference.Child (DBTable.Company.ToString ()).Child(companyID).Child (Parameters.responsables.ToString()).Child(responsableModel.responsableID).SetRawJsonValueAsync (json);
	}

	public UserModel CreateNewUser (string userID, string name)
	{
		UserModel user = new UserModel (userID, name);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	public ResponsableModel CreateNewResponsableToCompany (string companyID, string name)
	{
		string responsibleID = reference.Child (DBTable.Responsable.ToString ()).Push ().Key;
		ResponsableModel responsable = new ResponsableModel (responsibleID, name);
		string json = JsonUtility.ToJson (responsable);

		CreateTable (DBTable.Responsable, responsibleID, json);
		AddEmployeeToCompany(companyID, responsable);
		return responsable;
	}

	public void CreateTable(DBTable table, string tableID, string json) 
	{
		reference.Child (table.ToString ()).Child (tableID).SetRawJsonValueAsync (json);
	}

	public void GetAllResponsiblesFromCompany(String companyID)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Company.ToString ()).Child(companyID).Child(Parameters.responsables.ToString())
			.GetValueAsync().ContinueWith(task => {
			if (task.IsFaulted) {
				// Handle the error...
			}
			else if (task.IsCompleted) {
				DataSnapshot snapshot = task.Result;
				responsibles.Clear();
				foreach (var responsible in snapshot.Children)
				{
					string json = responsible.GetRawJsonValue();
					var mresponsible = JsonUtility.FromJson<ResponsableModel>(json);
					var a = 2;
				}

				foreach (var print in responsibles)
				{
					Debug.Log(print.name);
				}
			}
		});
	}

	void GetDaySchedule (string date)
	{
		FirebaseDatabase.DefaultInstance.GetReference (DBTable.Appoitments.ToString ()).Child("data").EqualTo (date)
			.ValueChanged += HandleValueChanged;

	}

	void HandleValueChanged(object sender, ValueChangedEventArgs args) {
		if (args.DatabaseError != null) {
			Debug.LogError(args.DatabaseError.Message);
			return;
		}
//		Debug.Log ("!!!!" + args.Snapshot.);
	}
}
