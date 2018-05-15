using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPageController : PageController
{
	public InputField idTest;
	public string homeScene;
	public string url = "http://servicodados.ibge.gov.br/api/v1/localidades/estados/33/municipios";

	void Start ()
	{
		Loading = false;
	}

	public void LoginTest ()
	{
		Loading = true;
		FireBaseManager.GetFireBaseInstance ().IsThereUser (idTest.text, delegate(bool isThereUser) {
			Debug.Log ("MyTag: Is there user with id: " + DataManager.userID + " - " + isThereUser);
			if (isThereUser) {
				DataManager.userID = idTest.text;
				Loading = false;
				ChangeScene ();
			} else {
				DataManager.userID = idTest.text;
				DataManager.CreateNewUserAndLogin (idTest.text, "userName" + idTest.text, idTest.text);
				Loading = false;
				ChangeScene ();
			}
		}, delegate(string error) {
			Debug.Log (error);
		});
	}

	public void FacebookLogin ()
	{
		Loading = true;
		FirebaseAuth.GetFireBaseAuthInstance ().FacebookLogin (delegate(string userId, string userName) {
			FireBaseManager.GetFireBaseInstance ().IsThereUser (userId, delegate(bool isThereUser) {
				Debug.Log ("MyTag: isThereUserWithID " + isThereUser);
				if (isThereUser) {
					DataManager.userID = userId;
					Loading = false;
					ChangeScene ();
				} else {
					DataManager.userID = userId;
					Loading = false;
					var dict = new Dictionary<string, object> ();
					dict.Add ("name", userName);
					dict.Add ("id", userId);
					PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.CompleteFacebookLoginPage, dict);
				}
			}, delegate(string error) {
				Loading = false;
				Error = true;
			});
//			DataManager.GetUserById (userId, delegate(UserModel user) {
//				if (user == null) {
//					Loading = false;
//					var dict = new Dictionary<string, object> ();
//					dict.Add ("name", userName);
//					dict.Add ("id", userId);
//					PageNav.GetPageNavInstance ().PushPageToStackWithArgs (PagesEnum.CompleteFacebookLoginPage, dict);
//				} else {
//					DataManager.LoadUserInfoAux (userId, delegate {
//						Loading = false;
//						ChangeScene ();
//					}, delegate(string error) {
//						Loading = false;
//						Error = true;
//					});
//				}
//			});
		}, delegate(string error) {
			Loading = false;
			Error = true;
		});
	}

	void ChangeScene ()
	{
		SceneManager.LoadSceneAsync (homeScene);
	}


	IEnumerator GetLocations ()
	{
		using (WWW www = new WWW (url)) {
			yield return www;

			if (www.error != null) {
				Debug.LogError ("Erro na chamada da api");
			} else {
				Debug.Log (www.text);
				Cities cities = JsonUtility.FromJson<Cities> (www.text);
				foreach (CityModel city in cities.cities) {
					Debug.Log (city.id + " " + city.nome);
				}
			}
		}
	}

}

[Serializable]
public class Cities
{
	public List<CityModel> cities;
}

[Serializable]
public class CityModel
{
	public string id;
	public string nome;
	public Dictionary <string, object> microrregiao;
}
