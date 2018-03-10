using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageCellControler : MonoBehaviour
{
	public Text message;
	public MessageModel messageModel;


	public void OnRemoveClick ()
	{
		DataManager.RemoveMessage (messageModel, delegate {
			GameObject.Destroy (gameObject);
		});
	}

	public static GameObject Instantiate (Transform CellPrefabTransform, string message, MessageModel messageModel)
	{
		GameObject go = GameObject.Instantiate (CellPrefabTransform).gameObject;
		var myMessageController = go.GetComponent<MessageCellControler> ();
		myMessageController.message.text = message;
		myMessageController.messageModel = messageModel;
		return go;
	}
}
