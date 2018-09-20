using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class EditProfilePopupController : PageController
{
	public GameObject editProfileContainer;
	public GameObject changePasswordContainer;

	public InputField userName;
	public GameObject userNameError;
	public InputField userPhone;
	public GameObject userPhoneError;


	public InputField newPassword;
	public GameObject newPasswordError;
	public InputField confirmPassword;
	public GameObject confirmPasswordError;

	public void Start()
	{
		userName.text = DataManager.currentUser.name;
		if (!string.IsNullOrEmpty(DataManager.currentUser.phone))
		{
			userPhone.text = DataManager.currentUser.phone;
		}
	}

	public void EnableEditProfileContainer()
	{
		editProfileContainer.SetActive(true);
		changePasswordContainer.SetActive(false);
	}

	public void EnableChangePasswordContainer()
	{
		editProfileContainer.SetActive(false);
		changePasswordContainer.SetActive(true);
	}

	public void ChangePassword()
	{
		int everyThingIsRight = 0;

		if (newPassword.text.Length < 6)
		{
			newPasswordError.SetActive(true);
		}
		else
		{
			newPasswordError.SetActive(false);
			everyThingIsRight++;
		}
		if (confirmPassword.text != newPassword.text)
		{
			confirmPasswordError.SetActive(true);
		}
		else
		{
			confirmPasswordError.SetActive(false);
			everyThingIsRight++;
		}

		if (everyThingIsRight < 2)
		{
			return;
		}

		Loading = true;
		FirebaseAuth.GetFireBaseAuthInstance().ChangePassword(newPassword.text, () =>
		{
			Loading = false;
			CloseModal();
			OpenSuccessPopup();
		}, (string error) =>
		{
			Loading = false;
			CloseModal();
			OpenErrorPopup();
		});
	}

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
