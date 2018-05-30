using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;

public class InternetReachability : MonoBehaviour
{
	bool isEnabled = true;

	//	void Update ()
	//	{
	//		if (isEnabled) {
	//			try {
	//				var status = Network.TestConnection ();
	//				Debug.Log ("Minha tag: " + status.ToString ());
	//			} catch {
	//				Debug.Log ("Error. Check internet connection!");
	//				PageNavFrameWork.PageNav.GetPageNavInstance ().SetInternetErrorVisibility (true);
	//				isEnabled = false;
	//			}
	////			if (Application.internetReachability == NetworkReachability.NotReachable) {
	////				Debug.Log ("Error. Check internet connection!");
	////				PageNavFrameWork.PageNav.GetPageNavInstance ().SetInternetErrorVisibility (true);
	////				isEnabled = false;
	////			}
	//		}


	//}

	public static bool IsConnected (string hostedURL = "http://www.google.com")
	{
		try {
			string HtmlText = GetHtmlFromUri (hostedURL);
			if (HtmlText == "")
				return false;
			else
				return true;
		} catch (IOException ex) {
			return false;
		}
	}

	static string GetHtmlFromUri (string resource)
	{
		string html = string.Empty;
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create (resource);
		try {
			using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse ()) {
				bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
				if (isSuccess) {
					using (StreamReader reader = new StreamReader (resp.GetResponseStream ())) {
						//We are limiting the array to 80 so we don't have
						//to parse the entire html document feel free to 
						//adjust (probably stay under 300)
						char[] cs = new char[80];
						reader.Read (cs, 0, cs.Length);
						foreach (char ch in cs) {
							html += ch;
						}
					}
				}
			}
		} catch {
			return "";
		}
		return html;
	}
}
