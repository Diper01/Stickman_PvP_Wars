using UnityEngine;
using UnityEditor;
public class ClearData : Editor
{
	[MenuItem("Tools/Clear PlayerPrefs")]
	private static void NewMenuOption()
	{
		PlayerPrefs.DeleteAll();
	}
}
