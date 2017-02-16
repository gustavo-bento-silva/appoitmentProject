using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IntArg : Object{
	public int i;
	public IntArg(int i){
		this.i = i;
	}
}

/// <summary>
///  Tabs control. Gets GameObjects Inside container, and use them as tabs.
/// </summary>
public class TabsControl : MonoBehaviour {

	public RectTransform[] Tabs;
	public int InitialTab = 0;
	public bool AutoRun = true;
	public iTween.EaseType easeType;
	public float AnimationTransitionTime = 0.5f;
	public RectTransform Content;
	private int _currentTab;
	public int CurrentTab{
		get{ 
			return _currentTab;
		}
		set{ 
			ChangeToTab (value);
		}
	}
	private bool initiated = false;
	
	void Start () {
		if(AutoRun){
			initiated = true;
			GetContentForTabs ();
			ArrangeTabsToInitialPositions ();
			ChangeToTab (InitialTab);
		}
	}
	
	
	void Update () {
	
	}

	void GetContentForTabs(){
		if(Tabs.Length > 0){
			return;
		}
		List<RectTransform> list = new List<RectTransform> (Tabs);
		foreach(RectTransform rt in Content){
			list.Add (rt);
		}
		Tabs = list.ToArray ();
	}

	void InitializeTabTransform(RectTransform rt){
		rt.anchorMax = new Vector2 (1,1);
		rt.anchorMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		rt.offsetMin = Vector2.zero;
	}

	void ArrangeTabsToInitialPositions(){
		Rect rect = Content.rect;
		float yTab = 0;
		foreach(RectTransform rt in Tabs){
			InitializeTabTransform (rt);
			rt.offsetMax = new Vector2 (yTab,0);
			rt.offsetMin = new Vector2 (yTab,0);
			yTab += Content.rect.width;
		}
	}

	/// <summary>
	/// Changes to page absolute(tabNumber%Tabs.Length). If 
	/// </summary>
	/// <param name="tabNumber">Tab number.</param>
	public void ChangeToTabAnimating(int tabNumber){
		if(!initiated){
			throw new UnityException ("The Tab Control need to be Initiated before changing pages.");
			return;
		}
		if (tabNumber < 0) {
			int tab = Tabs.Length + tabNumber % Tabs.Length;
			ChangeToTabAnimating (tab);
			return;
		} else if(tabNumber > Tabs.Length - 1){
			ChangeToTabAnimating (tabNumber%Tabs.Length);
			return;
		}
		float Yquantity = tabNumber * Content.rect.width;
		Content.gameObject.MoveTo (GameObjectExtension.Axis.X, - Yquantity, AnimationTransitionTime, easeType.ToString());
	}

	public void ChangeToTab(int tabNumber){
		Debug.Log ("Change to tab: " + tabNumber);
		if(!initiated){
			throw new UnityException ("The Tab Control need to be Initiated before changing pages.");
			return;
		}
		if (tabNumber < 0) {
			int tab = Tabs.Length + tabNumber % Tabs.Length;
			ChangeToTab (tab);
			return;
		} else if(tabNumber > Tabs.Length - 1){
			ChangeToTab (tabNumber%Tabs.Length);
			return;
		}
		float Yquantity = tabNumber * Content.rect.width;
		Content.localPosition = new Vector3 (- Yquantity,Content.localPosition.y,Content.localPosition.z);
//		Content.offsetMax = -new Vector2 (Yquantity, 0);
//		Content.offsetMin = -new Vector2 (Yquantity, 0);
	}

	public void InitiateTabs(){
		if (!initiated) {
			initiated = true;
			GetContentForTabs ();
			ArrangeTabsToInitialPositions ();
		} else {
			throw new UnityException ("The TabControl has already been initiated.");
		}
	}

	public void ChangePageFromObject(IntArg a){
		Debug.Log ("tab func");
		ChangeToTabAnimating (a.i);
	}

}
