using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using UnityEngine.UI;

public enum DBTable
{
	Appoitments,
	User,
	Responsable,
	Data
}
	
public class FireBaseManager : MonoBehaviour
{
	public static FireBaseManager _instance;

	string myProjectURL = "https://appointmentproject-a7233.firebaseio.com/";
	DatabaseReference reference;
	Firebase.Auth.FirebaseAuth auth;
	public Firebase.Auth.FirebaseUser user;

	public static FireBaseManager GetFireBaseInstance(){
		return _instance;
	}

	void Awake (){
		if (_instance == null) {
			_instance = this;
		}
	}

	void Start ()
	{
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (myProjectURL);
		reference = FirebaseDatabase.DefaultInstance.RootReference;

		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

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

	private void CreateNewAppoitment (string data, UserModel user, ResponsableModel responsable, string time)
	{
		string appoitmentID = reference.Child (DBTable.Appoitments.ToString ()).Push ().Key;
		AppointmentModel appointment = new AppointmentModel (data, user.userID, responsable.responsableID, time);

		user.appoitments[appoitmentID] = (object)responsable.responsableID;
		responsable.appoitments[appoitmentID] = (object)user.userID;

		string json = JsonUtility.ToJson (appointment);

		CreateTable (DBTable.Appoitments, appoitmentID, json);
		reference.Child (DBTable.User.ToString ()).Child (user.userID + "/appointments").SetValueAsync (user.appoitments);
		reference.Child (DBTable.Responsable.ToString ()).Child (responsable.responsableID + "/appointments").SetValueAsync (responsable.appoitments);
	}

	private UserModel CreateNewUser (string userID, string name)
	{
		UserModel user = new UserModel (userID, name);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	private ResponsableModel CreateNewResponsable (string responsableID, string name)
	{
		ResponsableModel responsable = new ResponsableModel (responsableID, name);
		string json = JsonUtility.ToJson (responsable);

		CreateTable (DBTable.Responsable, responsableID, json);
		return responsable;
	}

	/*private void AddAppoitmentForUser (UserModel user, ResponsableModel responsable, AppointmentModel appoitment)
	{

		reference.Child (table.ToString ()).Child (tableID).S;
	}*/

	private void CreateTable(DBTable table, string tableID, string json) 
	{
		reference.Child (table.ToString ()).Child (tableID).SetRawJsonValueAsync (json);
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
