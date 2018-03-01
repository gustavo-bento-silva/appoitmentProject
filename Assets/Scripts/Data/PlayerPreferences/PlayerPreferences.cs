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

	public static int TranslateMonth(string month)
	{
		switch(month)
		{
		case "Janeiro":
			return 1;
		case "Fevereiro":
			return 2;
		case "Março":
			return 3;
		case "Abril":
			return 4;
		case "Maio":
			return 5;
		case "Junho":
			return 6;
		case "Julho":
			return 7;
		case "Agosto":
			return 8;
		case "Setembro":
			return 9;
		case "Outubro":
			return 10;
		case "Novembro":
			return 11;
		case "Dezembro":
			return 12;
		}
		
		return 0;
	}
}
