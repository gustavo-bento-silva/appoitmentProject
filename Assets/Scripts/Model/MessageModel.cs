using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class MessageModel
{
	public string id;
	public string from;
	public string to;
	public string message;
	public string messageDate;
	public string messageType;
	public bool isNew;

	public MessageModel (string from, string to, string message, string messageDate, string messageType)
	{
		this.from = from;
		this.to = to;
		this.message = message;
		this.messageDate = messageDate;
		this.messageType = messageType;
		this.isNew = true;
	}

}
