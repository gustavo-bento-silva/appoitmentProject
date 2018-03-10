using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class MessageModel
{
	public string id;
	public string from;
	public string message;
	public string messageDate;

	public MessageModel (string from, string message, string messageDate)
	{
		this.from = from;
		this.message = message;
		this.messageDate = messageDate;
	}

}
