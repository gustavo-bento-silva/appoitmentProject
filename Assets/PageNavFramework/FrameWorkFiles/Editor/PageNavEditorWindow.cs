using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Reflection;
using System.Threading;

namespace PageNavFrameWork{
	public class PageNavEditorWindow : EditorWindow {
		private PageNavSettings settings = null;
		private GameObject pageTemplatePrefab = null;
		private static string PagesEnumPath = "Assets/PageNavFramework/FrameWorkFiles/Scripts";
		private const string PageControllerTemplate = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageControllerClassTemplate.txt";
		private static string PagesPath = "Assets/PageNavFramework/Pages";
		private static string PageTemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageTemplate.prefab";
		private static string PageNavtemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageNavTemplate/PageNav.prefab" ;
		string newPageName = "";
		public static List<string> PageNames = null;
		bool showPages = true;
		bool initiated = false;

		[MenuItem ("PageNav/Show PageNav Window",false,0)]
		static void Init () {
//			Get existing open window or if none, make a new one:
			PageNavEditorWindow window = (PageNavEditorWindow)EditorWindow.GetWindow (typeof (PageNavEditorWindow));
			window.Show();
		}
			
		static void CreatePageNavInScene(){
			var pageNav = GameObject.Find ("PageNav");
			if(pageNav){
				Debug.LogWarning ("There is already a PageNav object in the scene");
				return;
			}
			GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PageNavtemplatePath);
			pageNav = Instantiate (prefab);
			pageNav.name = prefab.name;
		}


		void OnGUI () {
			if(settings==null){
				this.LoadDefaultPageControllerSettings ();
				EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
				pageTemplatePrefab = AssetDatabase.LoadAssetAtPath<GameObject> (PageTemplatePath);
			}


			this.titleContent = new GUIContent ("PageNav Settings");
			GUIStyle myStyle = new GUIStyle();
			myStyle.fontSize = 20;
			myStyle.normal.textColor = GUI.skin.label.normal.textColor;
			GUILayout.Label ("  ",myStyle);
			GUILayout.Label ("  PageController Settings",myStyle);
			GUILayout.Label ("  ",myStyle);

			GUI.enabled = false;
			settings = (PageNavSettings)EditorGUILayout.ObjectField ("PageNav Settings",settings, typeof(PageNavSettings), false);
			GUI.enabled = true;

			if(GUILayout.Button("Create PageNav in open scene")){
				CreatePageNavInScene ();
			}

			ShowPagesSection ();
		}

		public static T CreateAsset<T> (string path) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T> ();

			AssetDatabase.CreateAsset (asset, path);

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			return asset;
		}

		void LoadDefaultPageControllerSettings ()
		{
			const string path = "Assets/PageNavFramework/FrameWorkFiles/PageNavSettings.asset";
			if (settings == null) {
				if (!File.Exists (path)) {
					if (!File.Exists ("Assets/Resources")) {
						Directory.CreateDirectory ("Assets/Resources");
					}
					settings = CreateAsset<PageNavSettings> (path);
				}
				else {
					settings = AssetDatabase.LoadAssetAtPath<PageNavSettings> (path);
				}
			}
			PagesEnumPath = settings.PagesEnumPath;
			PagesPath = settings.PagesPath;
			PageTemplatePath = settings.PageTemplatePath;
			PageNavtemplatePath = settings.PageNavtemplatePath;
		}

		bool ShowErrorBox = false;
		string errorMessage = "";
		void ShowPagesSection (){
			PageNames = settings.PageNames;
			showPages = EditorGUILayout.Foldout (showPages, "Pages");
			if (showPages) {
				EditorGUILayout.HelpBox ("The page`s name must contain only upper and lower case characters.",MessageType.Info,true);
				if(ShowErrorBox){
					EditorGUILayout.HelpBox (errorMessage,MessageType.Error,true);
				}
				GUILayout.BeginHorizontal ();
				newPageName = EditorGUILayout.TextField (newPageName);
				var oldColor = GUI.color;
				GUI.color = Color.green;
				GUIStyle whiteText = new GUIStyle (GUI.skin.button);
				whiteText.normal.textColor = Color.white;
				whiteText.active.textColor = Color.white;
				whiteText.hover.textColor = Color.white;
				whiteText.fixedWidth = GUI.skin.button.CalcSize(new GUIContent("Create new Page")).x;
				bool createNewPageButtonResponse = GUILayout.Button ("Create new Page");
				GUI.color = oldColor;
				if (createNewPageButtonResponse) {
					CreateNewPage (newPageName);
				}
				GUILayout.EndHorizontal ();

				int removeIndex = -1;
				GUILayout.BeginVertical ("box");
				for (int i = 0; i < PageNames.Count; i++) {
					removeIndex = ShowPageEntry (i);
					if (removeIndex >= 0) {
						RemovePage (removeIndex);
					}
				}
				GUILayout.EndVertical();

				ShowEnumEditButtons ();
			}
		}

		string formatedPageName = null;
		GameObject CreatePageTemplatePrefab(string newPageName){
			string directory = PagesPath + "/" + newPageName;
			Directory.CreateDirectory (directory);
			string prefabPath = directory + "/" + newPageName + ".prefab";
			AssetDatabase.CopyAsset (PageTemplatePath,prefabPath);
			AssetDatabase.ImportAsset (prefabPath);
			GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject> (prefabPath);
			EditorGUIUtility.PingObject (newPrefab);
			string template = File.ReadAllText (PageControllerTemplate);
			Regex rgx = new Regex ("[^a-zA-Z-]");
			formatedPageName = rgx.Replace (newPageName, "");
			template = template.Replace ("%classname%", formatedPageName+"Controller");
			string newScriptPath = directory+"/"+formatedPageName+"Controller.cs";
			File.WriteAllText (newScriptPath,template);
			AssetDatabase.ImportAsset (newScriptPath);
			finishedCompiling = false;
			EditorUtility.SetDirty (settings);
			return newPrefab;
		}

		static bool finishedCompiling = true;
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() {
			finishedCompiling = true;
		}
			
		bool pageCallBackGuard = true;
		public void Update(){
			if(!finishedCompiling){
				return;
			}
			if(!pageCallBackGuard){
				createPageCallback ();
				pageCallBackGuard = true;
			}
			
		}

		void createPageCallback(){
			Type newPageControllerType = GetTypeMe (formatedPageName +"Controller");
			settings.PagesPrefabs [settings.PagesPrefabs.Count - 1].AddComponent (newPageControllerType);
			EditorUtility.ClearProgressBar ();
		}

		void CreateNewPage (string newPagename)
		{
			if (!PageNames.Contains (newPageName)) {
				if (!string.IsNullOrEmpty (newPageName)) {
					Regex rgx = new Regex ("[^a-zA-Z]");
					if (!rgx.IsMatch (newPageName)) {
						if (!CheckForControllerExistance (newPageName)) {
							settings.PagesPrefabs.Add (CreatePageTemplatePrefab (newPageName));
							PageNames.Add (newPageName);
							settings.PagesCacheSettings.Add (true);
							EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
							pageCallBackGuard = false;
							EditorUtility.DisplayProgressBar ("Please wait...", "Waiting for editor to compile scripts", 0.5f);
							ShowErrorBox = false;
						}
					}
					else {
						ShowErrorBox = true;
						errorMessage = "The page name '" + newPageName + "', has invalid characters";
						Debug.LogError ("The page name '" + newPageName + "', has invalid characters");
					}
				}
				else {
					ShowErrorBox = true;
					errorMessage = "The pages name cannot be empty!";
					Debug.LogError ("The pages name cannot be empty!");
				}
			}
			else {
				ShowErrorBox = true;
				errorMessage = "There is already a page named '" + newPageName + "'";
				Debug.LogError ("There is already a page named '" + newPageName + "'");
			}
		}

		bool CheckForControllerExistance(string pageName){
			newPageName = newPageName.Replace (" ","_");
			Regex rgx = new Regex ("[^a-zA-Z-]");
			formatedPageName = rgx.Replace (newPageName, "");
			string controllerName = formatedPageName + "Controller";
			if(GetTypeMe(controllerName)!=null){
				ShowErrorBox = true;
				errorMessage = "There is already a PageController named '" + controllerName + "'";
				Debug.LogError ("There is already a PageController named '" + controllerName + "'");
				return true;
			}
			return false;
		}

		int ShowPageEntry (int index)
		{
			EditorGUILayout.BeginVertical ("box");
			int removeIndex = -1;
			EditorGUILayout.BeginHorizontal ();
//			var oldColor = GUI.color;
//			GUI.color = Color.red;
			GUIStyle whiteText = new GUIStyle (GUI.skin.button);
			var buttonSize = GUI.skin.button.CalcSize (new GUIContent("X"));
			Texture2D redTexture = CreateTexture (Color.red,buttonSize,false);
			Texture2D blackTexture = CreateTexture (Color.black,buttonSize,false);
			whiteText.normal.textColor = Color.white;
			whiteText.normal.background = redTexture;
			whiteText.active.textColor = Color.white;
			whiteText.active.background = blackTexture;
			whiteText.hover.textColor = Color.white;
			whiteText.hover.background = redTexture;
			whiteText.font = null;
			whiteText.margin = new RectOffset (2, 2, 2, 2);
			whiteText.fixedWidth = GUI.skin.button.CalcSize(new GUIContent("X")).x;
			bool removePage = GUILayout.Button ("X",whiteText);
//			GUI.color = oldColor;
			if (removePage) {
				if (finishedCompiling) {
					if (EditorUtility.DisplayDialog ("If you delete this Page, the Page's folder contents, including all files and scripts, will be deleted as well. Do you wish to continue?", "The folder '" + PagesPath + "/" + PageNames [index] + "/'", "I know what I'm doing.", "Cancel")) {
						removeIndex = index;
						AssetDatabase.DeleteAsset (PagesPath + "/" + PageNames [index].Replace (" ", "_"));
					}
				}
				else {
					EditorUtility.DisplayDialog ("Wait for the editor finish to compile scripts.", "", "Ok");
				}
			}
			var textDimensions = GUI.skin.label.CalcSize(new GUIContent(PageNames [index]));
			EditorGUIUtility.labelWidth = textDimensions.x;
			EditorGUILayout.LabelField (PageNames [index]);

			string newName = PageNames [index];
			Regex rgx = new Regex ("[^a-zA-Z -]");
			newName = rgx.Replace (newName, "");
			if (PageNames [index] != newName) {
				PageNames [index] = newName;
			}

			textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Cached"));
			EditorGUIUtility.labelWidth = textDimensions.x;
			settings.PagesCacheSettings [index] = EditorGUILayout.Toggle ("Is Cached",settings.PagesCacheSettings[index]);

			var style = new GUIStyle (GUI.skin.button);
			style.fixedWidth = GUI.skin.button.CalcSize (new GUIContent("Show Prefab")).x;
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			if(GUILayout.Button("Show Prefab",style)){
				EditorGUIUtility.PingObject (settings.PagesPrefabs[index]);
			}
			style = new GUIStyle (GUI.skin.button);
			style.fixedWidth = GUI.skin.button.CalcSize (new GUIContent("Instantiate Prefab")).x;
			if(GUILayout.Button(new GUIContent("Instantiate Prefab"),style)){
				GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab (settings.PagesPrefabs [index]);
				Undo.RegisterCreatedObjectUndo(newInstance,"Instantiated Page Prefab");
				newInstance.transform.SetAsLastSibling ();
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical();

			return removeIndex;
		}

		void RemovePage (int removeIndex)
		{
			PageNames.RemoveAt (removeIndex);
			settings.PagesPrefabs.RemoveAt (removeIndex);
			settings.PagesCacheSettings.RemoveAt (removeIndex);
			EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
			EditorUtility.SetDirty (settings);
		}

		void ShowEnumEditButtons ()
		{
			EditorGUILayout.BeginHorizontal ();
			if (PageNames.Count > 0) {
				if (GUILayout.Button ("Re-create PageEnum") && PageNames.Count >= 0) {
					EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
				}
			}
			if (GUILayout.Button ("Show Pages Enum")) {
				UnityEngine.Object enumFile = AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (PagesEnumPath + "/PagesEnum.cs");
				EditorGUIUtility.PingObject (enumFile);
			}
			EditorGUILayout.EndHorizontal ();
		}

		public void RecreateEnumFile(){
			EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
		}

		public Texture2D CreateTexture(Color color, Vector2 size, bool roundedCorners){
			Texture2D texture = null;
			if (roundedCorners) {
				texture = new Texture2D (10, 10);

				float cornerRadius = 5;
				float i = 0;
				for (float x = 0; x < texture.width; x += 1) {
					for (float y = 0; y < texture.height; y += 1) {
						texture.SetPixel ((int)x, (int)y, color);
						i++;
					}
				}
				texture = CreateRoundTexture (texture.height, texture.width, texture.height / 2, texture.height / 2, texture.width / 2, texture);
			} else {
				texture = new Texture2D (1, 1);
				texture.SetPixel (0, 0, color);
			}
			texture.Apply();
			return texture;
		}

		Texture2D CreateRoundTexture (int h, int w, float r, float cx, float cy, Texture2D sourceTex)
		{
			Color [] c = sourceTex.GetPixels (0, 0, sourceTex.width, sourceTex.height);
			Texture2D b = new Texture2D (h, w);
			for (int i = 0; i<(h*w); i++)
			{
				int y = Mathf.FloorToInt (((float)i) / ((float)w));
				int x = Mathf.FloorToInt (((float)i - ((float)(y * w))));
				if (r * r > (x - cx) * (x - cx) + (y - cy) * (y - cy)) {
					b.SetPixel (x, y, c [i]);
				} else if(r * r == (x - cx) * (x - cx) + (y - cy) * (y - cy)){
					b.SetPixel (x, y, Color.black);
				}else {
					b.SetPixel (x, y, Color.clear);
				}
			}
			b.Apply ();
			return b;
		}

		public static Type GetTypeMe( string TypeName )
		{

			// Try Type.GetType() first. This will work with types defined
			// by the Mono runtime, in the same assembly as the caller, etc.
			var type = Type.GetType( TypeName );

			// If it worked, then we're done here
			if( type != null )
				return type;

			// If the TypeName is a full name, then we can try loading the defining assembly directly
			if( TypeName.Contains( "." ) )
			{

				// Get the name of the assembly (Assumption is that we are using 
				// fully-qualified type names)
				var assemblyName = TypeName.Substring( 0, TypeName.IndexOf( '.' ) );

				// Attempt to load the indicated Assembly
				var assembly = Assembly.Load( assemblyName );
				if( assembly == null )
					return null;

				// Ask that assembly to return the proper Type
				type = assembly.GetType( TypeName );
				if( type != null )
					return type;

			}

			// If we still haven't found the proper type, we can enumerate all of the 
			// loaded assemblies and see if any of them define the type
			var currentAssembly = Assembly.GetExecutingAssembly();
			var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
			foreach( var assemblyName in referencedAssemblies )
			{

				// Load the referenced assembly
				var assembly = Assembly.Load( assemblyName );
				if( assembly != null )
				{
					// See if that assembly defines the named type
					type = assembly.GetType( TypeName );
					if( type != null )
						return type;
				}
			}

			// The type just couldn't be found...
			return null;

		}
	}

	[CustomPropertyDrawer(typeof(PagesEnum))]
	public class PagesEnumDrawer : PropertyDrawer {
		public static PageNavSettings settings = null;
		const string settingsPath = "Assets/PageNavFramework/FrameWorkFiles/PageNavSettings.asset";
		private const string PagesEnumPath = "Assets/PageNavFramework/FrameWorkFiles/Scripts";

		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			label = EditorGUI.BeginProperty(position, label, property);
			label.text = property.displayName;
			Rect pos = position;
			position = EditorGUI.PrefixLabel(pos, label);
			position.width /= 2;
			bool hasChanged = false;
			EditorGUI.BeginChangeCheck ();
			PagesEnum newEnum = (PagesEnum)EditorGUI.EnumPopup (position, ((PagesEnum)property.enumValueIndex));
			if(EditorGUI.EndChangeCheck()){
				hasChanged = true;
			}
			position.x += position.width;
			if(GUI.Button(position,"Show") && newEnum != PagesEnum.None){
				if(settings == null){
					settings = AssetDatabase.LoadAssetAtPath<PageNavSettings> (settingsPath);
				}
				EditorGUIUtility.PingObject (settings.PagesPrefabs[(int)newEnum - 1]);
			}
			if(property.enumValueIndex >= property.enumNames.Length){
				if (PageNavEditorWindow.PageNames != null) {
					EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNavEditorWindow.PageNames.ToArray (), PagesEnumPath);
				} else {
					Debug.LogWarning ("it is necessary to rebuild the Pages Enum");
					Debug.Log ("it is necessary to rebuild the Pages Enum");
				}
			}
			if(hasChanged){
				try{
					property.enumValueIndex = (int)newEnum;
				}catch( Exception e){
					property.enumValueIndex = 0 ;
				}
			}
			EditorGUI.EndProperty();
		}

	}


}