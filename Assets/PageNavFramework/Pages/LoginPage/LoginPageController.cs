using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class LoginPageController : PageController
{
	public string homeScene;
	public string url = "http://servicodados.ibge.gov.br/api/v1/localidades/estados/33/municipios";

	void Start ()
	{
		Loading = false;
		if (PlayerPreferences.userIsLogged) {
			ChangeScene ();
		}
	}

	void Update ()
	{

	}

	public void FacebookLogin ()
	{
		Loading = true;
		FirebaseAuth.GetFireBaseAuthInstance ().FacebookLogin (delegate(string userId) {
			DataManager.LoadUserInfoAux (userId, delegate {
				Loading = false;
				ChangeScene ();
			});
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
