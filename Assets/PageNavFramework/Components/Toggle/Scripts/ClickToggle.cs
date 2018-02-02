using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClickToggle : MonoBehaviour {

	public void OnToggleClick(){
		var value = this.GetComponentInParent<Slider>().value;
		value = 1 - value;
	}
}
