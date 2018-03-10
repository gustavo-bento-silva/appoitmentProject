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
		var date = DataManager.dateNewAppointment;
		int[] time = new int[2];
		time = GetTime ();
		DataManager.dateNewAppointment = new System.DateTime (date.Year, date.Month, date.Day, time [0], time [1], 0);
		var pageNav = PageNav.GetPageNavInstance ();
		var page = pageNav.GetPagePrefabByEnum (PagesEnum.ConfirmAppointmentPopup);
		pageNav.OpenModal (page);
	}

	//0 -> hour, 1 -> minute
	public int[] GetTime ()
	{
		string[] mStringTime = time.text.Split (':');
		int[] mtime = new int[2];
		mtime [0] = int.Parse (mStringTime [0]);
		mtime [1] = int.Parse (mStringTime [1]);
		return mtime;
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
