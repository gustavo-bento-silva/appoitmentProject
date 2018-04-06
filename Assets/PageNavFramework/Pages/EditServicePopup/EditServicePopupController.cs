using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using UnityEngine.UI;

public class EditServicePopupController : PageController
{
	ServicesProvidedModel serviceToUpdate;
	public InputField name;
	public GameObject nameError;
	public InputField price;
	public InputField duration;
	public GameObject durationError;

	void Start ()
	{
		name.text = serviceToUpdate.name;
		price.text = serviceToUpdate.price;
		duration.text = serviceToUpdate.duration.ToString ();
	}

	public void UpdateService ()
	{
		if (VerifyError ()) {
			Loading = true;
			serviceToUpdate.name = name.text;
			serviceToUpdate.price = price.text;
			serviceToUpdate.duration = float.Parse (duration.text);
			DataManager.UpdateService (serviceToUpdate, delegate() {
				Success = true;
				Loading = false;
				Constants.LoadHomePage ();
			}, delegate(string error) {
				Error = true;
				Loading = false;
			});
		}
	}

	public override void InstantiatedWithArgs (Dictionary<string,object> args)
	{
		foreach (var key in args.Keys) {
			serviceToUpdate = (ServicesProvidedModel)args [key];
		}
	}

	bool VerifyError ()
	{
		if (string.IsNullOrEmpty (name.text)) {
			nameError.SetActive (true);
			return false;
		} else {
			nameError.SetActive (false);
		}
		if (string.IsNullOrEmpty (duration.text)) {
			durationError.SetActive (true);
			return false;
		} else {
			durationError.SetActive (false);
		}
		return true;
	}
}
