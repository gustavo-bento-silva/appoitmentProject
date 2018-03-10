using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class ConfirmAppointmentPopupController : PageController
{
	public Text message;

	void Start ()
	{
		UpdateText ();
	}

	void Update ()
	{

	}

	void UpdateText ()
	{
		var date = DataManager.dateNewAppointment;
		var responsible = DataManager.currentResponsible.name;
		var service = DataManager.currentservice.name;
		var minute = " ";
		if (date.Minute == 0) {
			minute = "00";
		} else {
			minute = date.Minute.ToString ();
		}
		message.text = string.Format ("Deseja confirmar o agendamento de {0} com {1} no dia {2}/{3} as {4}:{5} horas?", service, responsible, date.Day, date.Month, date.Hour, minute);
	}

	public void OnCloseClick ()
	{
		CloseModal ();
	}

	public void OnYesClick ()
	{
		Loading = true;
		DataManager.CreateNewAppointmentToCurrentUser (delegate {
			Loading = false;
			DropAllPagesFromStack ();
			StartCoroutine (MyCloseModal ());
		}, delegate (string error) {
			Loading = false;
			CloseModal ();
		});
	}

	IEnumerator MyCloseModal ()
	{
		yield return new WaitForSeconds (0.3f);
		CloseModal ();
	}
}
