using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PageNavFrameWork{
	[System.Serializable]
	public class PageNavSettings : ScriptableObject {

		public string PagesEnumPath = "Assets/PageNavFramework/FrameWorkFiles/Scripts";
		public string PagesPath = "Assets/PageNavFramework/Pages";
		public string PageTemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageTemplate.prefab";
		public string PageNavtemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageNavTemplate/PageNav.prefab" ;

//		[HideInInspector]
		public List<string> PageNames;
//		[HideInInspector]
		public List<GameObject> PagesPrefabs;
//		[HideInInspector]
		public List<bool> PagesCacheSettings;
	}
}