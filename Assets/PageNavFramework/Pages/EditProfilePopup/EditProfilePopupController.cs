using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class EditProfilePopupController : PageController
{
	public InputField userName;
	public GameObject userNameError;
	public InputField userPhone;
	public GameObject userPhoneError;

	public void UpdateUserProfile()
	{

		int everyThingIsRight = 0;

		if (string.IsNullOrEmpty(userName.text))
		{
			userNameError.SetActive(true);
		}
		else
		{
			userNameError.SetActive(false);
			everyThingIsRight++;
		}
		if (userPhone.text.Length < 8)
		{
			userPhoneError.SetActive(true);
		}
		else
		{
			userPhoneError.SetActive(false);
			everyThingIsRight++;
		}

		if (everyThingIsRight < 2)
		{
			return;
		}
		Loading = true;
		FirebaseAuth.GetFireBaseAuthInstance().UpdateUserData(userName.text, userPhone.text, () =>
		{
			Loading = false;
			CloseModal();
			OpenSuccessPopup();
		});
	}
}
