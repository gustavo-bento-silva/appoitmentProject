#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PageNavFrameWork
{
	public class EnumFileCreator
	{
		public static void CreateOrOverwriteEnum (string enumName, string[] keys, string path)
		{

			#if UNITY_EDITOR
			// remove whitespace and minus
			string name = enumName;
			name = name.Replace ("-", "_");
			name = name.Replace (" ", "");
			string copyPath = Path.Combine (path, name + ".cs");
			Debug.Log ("Creating PagesEnum: " + copyPath);
			using (StreamWriter outfile = 
				       new StreamWriter (copyPath)) {
				outfile.WriteLine ("using UnityEngine;");
				outfile.WriteLine ("using System.Collections;");
				outfile.WriteLine ("");
				outfile.WriteLine ("namespace PageNavFrameWork{\n\tpublic enum " + enumName + " {");
				outfile.WriteLine ("\t\tNone = 0,");
				string keyName;
				for (int i = 0; i < keys.Length - 1; i++) {
					keyName = keys [i] + "=" + (i + 1) + ",";
					outfile.WriteLine ("\t\t" + keyName.Replace (" ", "_"));
				}
				if (keys.Length > 0) {
					keyName = keys [keys.Length - 1] + "=" + (keys.Length);
					outfile.WriteLine ("\t\t" + keyName.Replace (" ", "_"));
				}
				outfile.WriteLine ("\t}\n}");
			}//File written
			AssetDatabase.ImportAsset (copyPath);
			#endif
		}

		static void CreateScriptableObject ()
		{
			CreateAsset<EnumAssetHelper> ();
		}

		public static void CreateAsset<T> () where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T> ();

			#if UNITY_EDITOR
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") {
				path = "Assets";
			} else if (Path.GetExtension (path) != "") {
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + typeof(T).ToString () + ".asset");

			AssetDatabase.CreateAsset (asset, assetPathAndName);

			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
			#endif
		}
	}

	public class EnumAssetHelper : ScriptableObject
	{
		public List<string> Keys = new List<string> ();
	}
}