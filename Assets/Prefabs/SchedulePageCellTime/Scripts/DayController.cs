using UnityEngine;
using System.Collections;
using PageNavFrameWork;
using UnityEngine.UI;

public class DayController : MonoBehaviour
{

	public Text description;
	public Text time;
	public Image background;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void OnCellClick ()
	{
		var pageNav = PageNav.GetPageNavInstance ();
//		pageNav.OpenModal(pageNav.GetPagePrefabByEnum(PagesEnum.ExamplePage));
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, string time, string description, bool isFree = true)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var dayControler = go.GetComponent<DayController> ();
		if (!isFree) {
			dayControler.background.color = Color.red;
			go.GetComponent<Image> ().color = Color.red;
		}
		dayControler.description.text = description;
		dayControler.time.text = time;
		return go;
	}
}
