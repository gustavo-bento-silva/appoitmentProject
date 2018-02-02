using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DayController : MonoBehaviour
{

	public Text description;
	public Text time;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static GameObject Instantiate(Transform CellPrefabTransform, Transform listContentReference, string time, string description)
	{
		GameObject go = GameObject.Instantiate(CellPrefabTransform).gameObject;
		var dayControler = go.GetComponent<DayController>();
		go.transform.SetParent(listContentReference, false);
		dayControler.description.text = description;
		dayControler.time.text = time;
		return go;
	}
}
