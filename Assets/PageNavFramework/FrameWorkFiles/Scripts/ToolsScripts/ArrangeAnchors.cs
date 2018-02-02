using UnityEngine;
using System.Collections;

public class ArrangeAnchors : MonoBehaviour {
	
	void OnDrawGizmos() {
		RectTransform rt = transform as RectTransform;
		RectTransform parent = rt.parent as RectTransform;
		Vector2 pivotParent = new Vector2(parent.pivot.x, parent.pivot.y);
		parent.pivot = Vector2.zero;
		Rect rectrt = rt.rect;
		float width = rectrt.width;
		float heigth = rectrt.height;
		float x = rt.localPosition.x;
		float y =  rt.localPosition.y;
		Rect prect = parent.rect;
		rt.anchorMin = new Vector2((x - width/2)/prect.width,(y - heigth/2)/prect.height);
		rt.anchorMax = new Vector2((x + width/2)/prect.width,(y + heigth/2)/prect.height);
		rt.offsetMax = Vector2.zero;
		rt.offsetMin = Vector2.zero;
		parent.pivot = pivotParent;
		DestroyImmediate (this);
	}

}
