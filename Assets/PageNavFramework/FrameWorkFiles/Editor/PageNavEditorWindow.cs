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
		public static PageNavSettings Settings = null;
		private GameObject pageTemplatePrefab = null;
        public static string PathRoot
        {
            get
            {
                if (_PathRoot == null)
                {
                    string[] res = System.IO.Directory.GetFiles(".", "PageNavEditorWindow.cs", SearchOption.AllDirectories);
                    if (res.Length == 0)
                    {
                        Debug.LogError("Could not find PageNav Framework files.");
                        return null;
                    }
                    if (res.Length > 1)
                    {
                        Debug.LogError("There are two files named 'PageNavEditorWindow.cs'. Maybe you imported the PageNavFramework twice, in diferent paths.");
                        return null;
                    }
                    string path = res[0];
                    path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(path)));
                    path = path.Substring(2);
                    _PathRoot = path;
                    return path;
                }
                else
                {
                    return _PathRoot;
                }
            }
        }
        private static string _PathRoot = null;

        private static string PagesEnumPath = "Assets/PageNavFramework/FrameWorkFiles/Scripts";
		private static string PageControllerTemplate = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageControllerClassTemplate.txt";
		private static string PagesPath = "Assets/PageNavFramework/Pages";
		private static string PageTemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageTemplate.prefab";
		private static string PageNavtemplatePath = "Assets/PageNavFramework/FrameWorkFiles/Templates/PageNavTemplate/PageNav.prefab" ;
		string newPageName = "";
		public static List<string> PageNames = null;
		bool showPages = true;
		bool initiated = false;
		public PageArgs pageArgs = new PageArgs();

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
			Undo.RegisterCreatedObjectUndo(pageNav,"Created PageNav in scene");
			pageNav.name = prefab.name;
			pageNav.GetComponent<PageNav> ().settings = Settings;
		}

		private Vector2 scrollPos;
		void OnGUI () {
            LoadFilePaths();
            if (settings==null){
				this.LoadDefaultPageControllerSettings ();
				EnumFileCreator.CreateOrOverwriteEnum ("PagesEnum", PageNames.ToArray (), PagesEnumPath);
				pageTemplatePrefab = AssetDatabase.LoadAssetAtPath<GameObject> (PageTemplatePath);
			}
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			PageNavEditorWindow.Settings = settings;
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

			EditorGUILayout.EndScrollView ();
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
            LoadFilePaths();
            string path = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/PageNavSettings.asset";
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
			PageNavEditorWindow.Settings = settings;
		}

        void LoadFilePaths()
        {
            PageNavEditorWindow.PagesEnumPath = PageNavEditorWindow.PathRoot+"/FrameWorkFiles/Scripts";
            PageNavEditorWindow.PageControllerTemplate = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/Templates/PageControllerClassTemplate.txt";
            PageNavEditorWindow.PagesPath = PageNavEditorWindow.PathRoot + "/Pages";
            PageNavEditorWindow.PageTemplatePath = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/Templates/PageTemplate.prefab";
            PageNavEditorWindow.PageNavtemplatePath = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/Templates/PageNavTemplate/PageNav.prefab";
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
			PageController comp = (PageController)settings.PagesPrefabs [settings.PagesPrefabs.Count - 1].AddComponent (newPageControllerType);
             comp.Title = settings.PageNames[settings.PagesPrefabs.Count - 1];
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
							var pageArgs = new PageArgs ();
							pageArgs.PageToCreate = (PagesEnum)settings.PagesCustomArguments.Count+1;
							settings.PagesCustomArguments.Add (pageArgs);
							settings.PagesTitles.Add (newPagename);
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
			var textDimensions = GUI.skin.label.CalcSize(new GUIContent("Title "));
			float originalLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = textDimensions.x;
			EditorGUILayout.LabelField (PageNames [index]);

			string newName = PageNames [index];
			Regex rgx = new Regex ("[^a-zA-Z -]");
			newName = rgx.Replace (newName, "");
			if (PageNames [index] != newName) {
				PageNames [index] = newName;
			}
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUI.BeginChangeCheck ();
			settings.PagesTitles[index] = EditorGUILayout.TextField ("Title",settings.PagesTitles[index]);
			if(EditorGUI.EndChangeCheck()){
				settings.PagesPrefabs[index].GetComponent<PageController>().Title = settings.PagesTitles[index];
			}
			textDimensions = GUI.skin.label.CalcSize(new GUIContent("Is Cached"));
			EditorGUIUtility.labelWidth = textDimensions.x;
			settings.PagesCacheSettings [index] = EditorGUILayout.Toggle ("Is Cached",settings.PagesCacheSettings[index]);
			EditorGUIUtility.labelWidth = originalLabelWidth;
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();

			var style = new GUIStyle (GUI.skin.button);
			style.fixedWidth = GUI.skin.button.CalcSize (new GUIContent("Show Prefab")).x;
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
			var settingsSerialized = new SerializedObject (settings);
			SerializedProperty PagesCustomArgumentsProperty = settingsSerialized.FindProperty ("PagesCustomArguments");
			SerializedProperty pageArgProperty = PagesCustomArgumentsProperty.GetArrayElementAtIndex (index);

			EditorGUILayout.PropertyField (pageArgProperty,new GUIContent("Default Page Args"),false);
			settingsSerialized.ApplyModifiedProperties ();
			EditorGUILayout.EndVertical();

			return removeIndex;
		}

		void RemovePage (int removeIndex)
		{
			PageNames.RemoveAt (removeIndex);
			settings.PagesPrefabs.RemoveAt (removeIndex);
			settings.PagesCacheSettings.RemoveAt (removeIndex);
			settings.PagesCustomArguments.RemoveAt (removeIndex);
			settings.PagesTitles.RemoveAt (removeIndex);
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
		string settingsPath = "/FrameWorkFiles/PageNavSettings.asset";
		string PagesEnumPath = "/FrameWorkFiles/Scripts";

		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            settingsPath = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/PageNavSettings.asset";
            PagesEnumPath = PageNavEditorWindow.PathRoot + "/FrameWorkFiles/Scripts";
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
				property.enumValueIndex = 0;
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

	[CustomPropertyDrawer(typeof(PageArgs))]
	public class PageArgsHolderDrawer : PropertyDrawer {

		float rows = 0;
		const float minRows = 0;
		float extraHeight = 0;
		Rect pos;
		float lineHeigth;
		Rect OriginalPos;
		float spaceBetweenElements = 0;

		void InitializePosition (Rect position)
		{
			OriginalPos = position;
			this.pos = position;
			lineHeigth = GUI.skin.label.CalcSize (new GUIContent ("Label")).y;
			pos.height =lineHeigth;
			rows = minRows;
		}

		// Draw the property inside the given rect
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			var expandEntirePropertyBool = property.FindPropertyRelative ("_expandedEditorEntireProperty");
			var expandedBool = property.FindPropertyRelative ("_expandedEditor");
			var pageToCreateProperty = property.FindPropertyRelative ("PageToCreate");
			var ArgumentList = property.FindPropertyRelative ("ArgumentList");
			int listSize = ArgumentList.FindPropertyRelative ("Array.size").intValue;

			InitializePosition (position);
			float originalY;
			label = EditorGUI.BeginProperty(position, label, property);
			expandEntirePropertyBool.boolValue = EditorGUI.Foldout (NextLineLabel (), expandEntirePropertyBool.boolValue, label.text);
			if (expandEntirePropertyBool.boolValue) {
				EditorGUI.indentLevel++;
				EditorGUI.LabelField (NextLineLabel (), "Page To Create");
				pos.y -= lineHeigth;
				EditorGUI.BeginChangeCheck ();
				pageToCreateProperty.enumValueIndex = (int)(PagesEnum)EditorGUI.EnumPopup (PositionInX (GUI.skin.label.CalcSize (new GUIContent ("Page To Create")).x + 5, 0.6f, true), (PagesEnum)pageToCreateProperty.enumValueIndex);
				if(EditorGUI.EndChangeCheck() && pageToCreateProperty.enumValueIndex > 0){
					var settings = PageNavEditorWindow.Settings;
					var settingsSerialized = new SerializedObject (settings);
					SerializedProperty pageArgsList = settingsSerialized.FindProperty ("PagesCustomArguments");
					SerializedProperty defaultPageArgs = pageArgsList.GetArrayElementAtIndex (pageToCreateProperty.enumValueIndex-1);
					SerializedProperty defaultPageArgsArgList = defaultPageArgs.FindPropertyRelative ("ArgumentList");
					ArgumentList.ClearArray ();
					if (defaultPageArgsArgList != null) {
						for (int i = 0; i < defaultPageArgsArgList.FindPropertyRelative ("Array.size").intValue; i++) {
							ArgumentList.InsertArrayElementAtIndex (i);
							var element = ArgumentList.GetArrayElementAtIndex (i); 
							element.FindPropertyRelative ("name").stringValue = defaultPageArgsArgList.GetArrayElementAtIndex (i).FindPropertyRelative ("name").stringValue;
							element.FindPropertyRelative ("type").enumValueIndex = defaultPageArgsArgList.GetArrayElementAtIndex (i).FindPropertyRelative ("type").enumValueIndex;
						}
					}
				}
				pos.y += lineHeigth;
				expandedBool.boolValue = EditorGUI.Foldout (NextLineLabel (), expandedBool.boolValue, "Arguments List:");
				if (expandedBool.boolValue && pageToCreateProperty.enumValueIndex>0) {
					List<int> removeIndexes = new List<int> ();
					listSize = ArgumentList.FindPropertyRelative ("Array.size").intValue;
					for (int i = 0; i < listSize; i++) {
						pos.height = 3 * lineHeigth;
						pos.y += 5;
						NextLineLabel ();
						if (Event.current.type == EventType.Repaint) {
							GUI.skin.box.Draw (new Rect (pos.x + 25, pos.y - lineHeigth - 5, pos.width * 0.9f, pos.height + 10), "", false, false, false, false);
						}
						pos.height = lineHeigth;
						pos.y -= lineHeigth;
						var nameProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("name");
						nameProperty.stringValue = EditorGUI.TextField (PositionInX (pos.x, 0.6f, true), nameProperty.stringValue);

						if (GUI.Button (PositionInX (pos.x + 0.6f * pos.width + 2, 0.3f, true), "Remove")) {
							removeIndexes.Add (i);
						}

						NextLineLabel ();
						pos.y += 1;
						var argTypeProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("type");
						argTypeProperty.enumValueIndex = (int)(PageArgs.PageArgType)EditorGUI.EnumPopup (PositionInX (pos.x, 0.9f, true), (PageArgs.PageArgType)argTypeProperty.enumValueIndex);
						NextLineLabel ();
						pos.y += 1;
						SerializedProperty valueProperty = null;
						switch ((PageArgs.PageArgType)argTypeProperty.enumValueIndex) {
						case PageArgs.PageArgType.objectType:
							valueProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("objectValue");
							EditorGUI.PropertyField (PositionInX (pos.x, 0.9f, true), valueProperty, new GUIContent ("Object"));
							break;
						case PageArgs.PageArgType.intType:
							valueProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("intValue");
							EditorGUI.PropertyField (PositionInX (pos.x, 0.9f, true), valueProperty, new GUIContent ("Int"));
							break;
						case PageArgs.PageArgType.stringType:
							valueProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("stringValue");
							EditorGUI.PropertyField (PositionInX (pos.x, 0.9f, true), valueProperty, new GUIContent ("String"));
							break;
						case PageArgs.PageArgType.boolType:
							valueProperty = ArgumentList.GetArrayElementAtIndex (i).FindPropertyRelative ("boolValue");
							EditorGUI.PropertyField (PositionInX (pos.x, 0.9f, true), valueProperty, new GUIContent ("Bool"));
							break;
						}
						pos.y += 20;
					}
					if (GUI.Button (new Rect (pos.x + 25, pos.y, pos.width * 0.9f, lineHeigth+2), "Add Argument")) {
						ArgumentList.InsertArrayElementAtIndex (listSize);
						SerializedProperty argument = ArgumentList.GetArrayElementAtIndex (listSize);
						argument.FindPropertyRelative ("name").stringValue = "Argument " + listSize;
						argument.FindPropertyRelative ("type").enumValueIndex = 0;

					}
					pos.y += lineHeigth;
					foreach (int index in removeIndexes) {
						ArgumentList.DeleteArrayElementAtIndex (index);
					}
				}
				EditorGUI.indentLevel--;
				if (Event.current.type == EventType.Repaint && EditorGUIUtility.isProSkin) {
					GUI.skin.box.Draw (new Rect (OriginalPos.x, OriginalPos.y + lineHeigth, OriginalPos.width, base.GetPropertyHeight (property, label) * (rows - 1) + extraHeight), "", false, false, false, false);
				}
			}
			EditorGUI.EndProperty();
		}

		public Rect NextLineLabel(int lineAmount = 1){
			rows++;
			Rect oldPos = new Rect (pos);
			pos.y += lineHeigth * lineAmount;
			return oldPos;
		}

		public Rect PositionInX(float x,float width,bool fraction = false){
			Rect oldPos = new Rect (pos);
			oldPos.x += x;
			if(fraction){
				oldPos.width *= width;
			}else{
				oldPos.width = width;
			}
			return oldPos;
		}

		public void CalculateHeigthVariables(SerializedProperty property){
			var expandEntirePropertyBool = property.FindPropertyRelative ("_expandedEditorEntireProperty");
			var pageToCreate = property.FindPropertyRelative ("PageToCreate");
			var expandedBool = property.FindPropertyRelative("_expandedEditor");
			rows = 1;
			if (expandEntirePropertyBool.boolValue) {
				rows += 2;
				extraHeight += 10;
			} else {
				expandedBool.boolValue = false;
			}
			extraHeight = 0;
			var ArgumentList = property.FindPropertyRelative ("ArgumentList");
			int listSize = ArgumentList.FindPropertyRelative("Array.size").intValue;
			if (expandedBool.boolValue && expandEntirePropertyBool.boolValue && pageToCreate.enumValueIndex > 0) {
				extraHeight += 11 * listSize;
				rows += 3 * listSize;
				spaceBetweenElements = 20;
			} else {
				spaceBetweenElements = 0;
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
			lineHeigth = GUI.skin.label.CalcSize (new GUIContent ("Label")).y;
			CalculateHeigthVariables (property);
			var customHeigth = base.GetPropertyHeight (property, label) * rows + extraHeight + spaceBetweenElements;
			return customHeigth;
		}

	}

	[CustomEditor(typeof(PageArgsHolder))]
	public class PageArgsHolderCustomEditor : Editor {
		// Draw the property inside the given rect
		public bool argumentsListFoldout = false;
		public PageArgs pageArgsHolder;
		List<PageArgs.ArgItemHolder> tempList;


		public override void OnInspectorGUI()
		{
			pageArgsHolder =  ((PageArgsHolder)serializedObject.targetObject).pageArgs;
			EditorGUI.BeginChangeCheck ();
			pageArgsHolder.PageToCreate = (PagesEnum)EditorGUILayout.EnumPopup ("Page To Create: ",pageArgsHolder.PageToCreate);
			if(EditorGUI.EndChangeCheck()){
				pageArgsHolder.ArgumentList.Clear ();
				pageArgsHolder.ArgumentList = new List<PageArgs.ArgItemHolder> (PageNavEditorWindow.Settings.PagesCustomArguments[(int)pageArgsHolder.PageToCreate-1].ArgumentList);
			}
			argumentsListFoldout = EditorGUILayout.Foldout (argumentsListFoldout,"Arguments List:");
			int removeIndex = -1;
			tempList = new List<PageArgs.ArgItemHolder> (pageArgsHolder.ArgumentList);
			if(argumentsListFoldout && pageArgsHolder.PageToCreate!=PagesEnum.None){
				for(int index = 0; index < pageArgsHolder.ArgumentList.Count; index++){
					removeIndex =  DrawArgItem (pageArgsHolder.ArgumentList[index],index);
					if(removeIndex >= 0){
						tempList.Remove (pageArgsHolder.ArgumentList[index]);
					}
				}
				if (GUILayout.Button ("Add Argument")) {
					var lala = new PageArgs.ArgItemHolder ();
					lala.name = "Argument "+pageArgsHolder.ArgumentList.Count;
					tempList.Add (lala);
				}
				pageArgsHolder.ArgumentList = tempList;
			}

		}

		private int DrawArgItem(PageArgs.ArgItemHolder argItem, int index){
			int returnIndex = -1;
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.BeginHorizontal ();
			argItem.name = EditorGUILayout.TextField (argItem.name);
			if(GUILayout.Button("Remove")){
				tempList.Remove (argItem);
			}
			EditorGUILayout.EndHorizontal ();
			argItem.type = (PageArgs.PageArgType)EditorGUILayout.EnumPopup ("Type",argItem.type);

			switch(argItem.type){
			case PageArgs.PageArgType.objectType:
				argItem.objectValue = (UnityEngine.Object)EditorGUILayout.ObjectField ("Value", argItem.objectValue, typeof(UnityEngine.Object), true);
				break;
			case PageArgs.PageArgType.intType:
				argItem.intValue = EditorGUILayout.IntField ("Value",argItem.intValue);
				break;
			case PageArgs.PageArgType.stringType:
				argItem.stringValue = EditorGUILayout.TextField ("Value",argItem.stringValue);
				break;
			case PageArgs.PageArgType.boolType:
				argItem.boolValue = EditorGUILayout.Toggle ("Value",argItem.boolValue);
				break;
			}

			EditorGUILayout.EndVertical ();
			return returnIndex;
		}

	}

}