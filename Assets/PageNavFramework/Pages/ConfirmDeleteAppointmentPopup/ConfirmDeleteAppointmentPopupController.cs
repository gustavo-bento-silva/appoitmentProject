using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConfirmDeleteAppointmentPopupController : PageController
{
	public Text message;
	AppointmentModel appointment;

	void Start ()
	{
		string clientOrResponsible = "";
		if (DataManager.currentUser.userType == Constants.UserType.Responsible.ToString ()) {
			clientOrResponsible = appointment.userName;
		} else {
			clientOrResponsible = appointment.responsibleName;
		}
		message.text = string.Format ("Deseja desmarcar o agendamento de {0} com {1} no dia {2} as {3}:{4}h ?", appointment.description, clientOrResponsible, appointment.data, appointment.hour, appointment.minute.ToString ("00"));
	}

	public void OnYesClick ()
	{
		Loading = true;
		DataManager.RemoveAppointmentFromUser (appointment, delegate {
			Loading = false;
			CloseModal ();
			Constants.LoadHomePage ();
		}, delegate(string error) {
			Loading = false;
			Error = true;
			CloseModal ();
			Constants.LoadHomePage ();
		});
	}

	public void OnNoClick ()
	{
		Constants.LoadHomePage ();
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			appointment = (AppointmentModel)args [key];
		}
	}
}
