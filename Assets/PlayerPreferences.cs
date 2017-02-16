using UnityEngine;
using System.Collections;

public class PlayerPreferences : MonoBehaviour {

	public static int initialTime = 7;
	public static int endTime = 17;
	public static int initialDayOfWeek = 1; //Monday
	public static int endDayOfWeek = 5; //Friday
	public static bool oneInOneHour = false;

	public static string TranslateDay(int day)
	{
		switch(day)
		{
		case 0:
			return "Domingo";
		case 2:
			return "Segunda-Feira";
		case 3:
			return "Terça-Feira";
		case 4:
			return "Quarta-Feira";
		case 5:
			return "Quinta-Feira";
		case 6:
			return "Sexta-Feira";
		case 7:
			return "Sábado";
		}
		return "";
	}

	public static string TranslateMonth(int month)
	{
		switch(month)
		{
			case 1:
				return "Janeiro";
			case 2:
				return "Fevereiro";
			case 3:
				return "Março";
			case 4:
				return "Abril";
			case 5:
				return "Maio";
			case 6:
				return "Junho";
			case 7:
				return "Julho";
			case 8:
				return "Agosto";
			case 9:
				return "Setembro";
			case 10:
				return "Outubro";
			case 11:
				return "Novembro";
			case 12:
				return "Dezembro";
		}
		
		return "";
	}
}
