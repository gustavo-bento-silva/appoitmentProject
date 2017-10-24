using UnityEngine;
using System.Collections;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public enum DBTable
{
	Appoitments,
	User,
	Responsable,
	Data
}
	
public class FireBaseManager : MonoBehaviour
{

	string myProjectURL = "https://appointmentproject-a7233.firebaseio.com/";
	DatabaseReference reference;

	// Use this for initialization
	void Start ()
	{
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl (myProjectURL);
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		UserModel user = CreateNewUser ("Gustavo");
		ResponsableModel responsable = CreateNewResponsable ("Geber");

		CreateNewAppoitment ("2017-10-24", user, responsable, "08:00");

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

	private UserModel CreateNewUser (string name)
	{
		string userID = reference.Child (DBTable.User.ToString ()).Push ().Key;
		UserModel user = new UserModel (userID, name);
		string json = JsonUtility.ToJson (user);

		CreateTable (DBTable.User, userID, json);
		return user;
	}

	private ResponsableModel CreateNewResponsable (string name)
	{
		string responsableID = reference.Child (DBTable.Responsable.ToString ()).Push ().Key;
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


//	.GetValueAsync ().ContinueWith ((System.Threading.Tasks.Task<DataSnapshot> arg) => {
//		if (arg.IsFaulted) {
//			Debug.Log ("Deu ruim!");
//		} else if (arg.IsCompleted) {
//
//			Debug.Log ("Deu bom!");
//			//				DataSnapshot snapshot = arg.Result;
//
//			//				foreach (var appointment in snapshot.Children) {
//			//					var key = appointment.Value.ToString ();
//			//					var mDate = appointment.Child ("data").Value.ToString ();
//			//					var responsable = appointment.Child ("responsable").Value.ToString ();
//			//					var time = appointment.Child ("time").Value.ToString ();
//			//				}
//		}
//	});