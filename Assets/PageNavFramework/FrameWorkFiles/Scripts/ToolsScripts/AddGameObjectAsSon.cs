using UnityEngine;
using System.Collections;

public class AddGameObjectAsSon : MonoBehaviour {

	public GameObject prefab = null;

	void OnDrawGizmos() {
		if(prefab==null){
			return;
		}
		GameObject obj = Instantiate (prefab);
		obj.transform.SetParent (this.transform,false);
		DestroyImmediate (this);
	}
}
