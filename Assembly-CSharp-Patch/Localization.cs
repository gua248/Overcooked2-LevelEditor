using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LevelEditor;
using Steamworks;
using UnityEngine;

public class Localization : MonoBehaviour
{
	[Serializable]
	private class DLCSerializedLocalizationData : DLCSerializedData<LocalizationData>
	{
	}

	private const string PRECODE = "<color=red>";

	private const string POSTCODE = "</color>";

	private const string A_BUTTON = "\u0080";

	private const string B_BUTTON = "\u0081";

	private const string X_BUTTON = "\u0082";

	private const string Y_BUTTON = "\u0082";

	private static int m_SChineseHashCode = "schinese".GetHashCode();

	private static int m_TChineseHashCode = "tchinese".GetHashCode();

	private static int m_KoreanHashCode = "koreana".GetHashCode();

	private static int m_BrazilianHashCode = "brazilian".GetHashCode();

	private static int m_EnglishHashCode = "english".GetHashCode();

	private static int m_FrenchHashCode = "french".GetHashCode();

	private static int m_ItalianHashCode = "italian".GetHashCode();

	private static int m_GermanHashCode = "german".GetHashCode();

	private static int m_SpanishHashCode = "spanish".GetHashCode();

	private static int m_JapaneseHashCode = "japanese".GetHashCode();

	private static int m_PolishHashCode = "polish".GetHashCode();

	private static int m_RussianHashCode = "russian".GetHashCode();

	private static string[] m_Languages = null;

	private static List<string> m_LanguageFiles = new List<string>();

	private static Hashtable m_stringTable = new Hashtable();

	private static bool m_bIsLoaded = false;

	private static bool m_bIsLoading = false;

	[SerializeField]
	private DLCSerializedLocalizationData m_LocalizationData = new DLCSerializedLocalizationData();

	private static readonly string[] c_localisationDataPaths = new string[12]
	{
		"Assets/Data/Localization/Localization - {0}", "Assets/DownloadableContent/DLC02/DLC_Assets/Data/Localization/Localization_DLC02 - {0}", "Assets/DownloadableContent/DLC03/DLC_Assets/Data/Localization/Localization_DLC03 - {0}", "Assets/DownloadableContent/DLC04/DLC_Assets/Data/Localization/Localization_DLC04 - {0}", "Assets/DownloadableContent/DLC05/DLC_Assets/Data/Localization/Localization_DLC05 - {0}", "Assets/DownloadableContent/DLC06/DLC_Assets/Data/Localization/Localization_DLC06 - {0}", "Assets/DownloadableContent/DLC07/DLC_Assets/Data/Localization/Localization_DLC07 - {0}", "Assets/DownloadableContent/DLC08/DLC_Assets/Data/Localization/Localization_DLC08 - {0}", "Assets/DownloadableContent/DLC09/DLC_Assets/Data/Localization/Localization_DLC09 - {0}", "Assets/DownloadableContent/DLC10/DLC_Assets/Data/Localization/Localization_DLC10 - {0}",
		"Assets/DownloadableContent/DLC11/DLC_Assets/Data/Localization/Localization_DLC11 - {0}", "Assets/DownloadableContent/DLC13/Assets/Data/Localization/Localization_DLC13 - {0}"
	};

	private void Awake()
	{
		TextAsset[] array = new TextAsset[0];
		LocalizationData[] allData = m_LocalizationData.AllData;
		allData = allData.AllRemoved_Predicate((LocalizationData x) => x == null);
		for (int num = 0; num < allData.Length; num++)
		{
			array = array.Union(allData[num].m_Localizations);
		}
		LoadDictionarys(false, array);
	}

	public static bool LoadDictionarys(bool reload, TextAsset[] localisationFiles = null)
	{
		if (reload)
		{
			m_bIsLoaded = false;
			m_bIsLoading = false;
			m_Languages = null;
			m_stringTable.Clear();
		}
		if (m_bIsLoaded || m_bIsLoading)
		{
			return true;
		}
		m_bIsLoading = true;
		for (int i = 0; i < c_localisationDataPaths.Length; i++)
		{
			string format = c_localisationDataPaths[i];
			AddLanguageFile(string.Format(format, "Shared"), localisationFiles);
			AddLanguageFile(string.Format(format, "PC"), localisationFiles);
		}
		m_bIsLoading = false;
		m_bIsLoaded = true;
		return true;
	}

	private static bool AddLanguageFile(string path, TextAsset[] localisationFiles = null)
	{
		if (m_LanguageFiles.Contains(path))
		{
			return false;
		}
		m_LanguageFiles.Add(path);
		TextAsset text = null;
		if (localisationFiles != null)
		{
			int num = localisationFiles.FindIndex_Predicate((TextAsset x) => path.EndsWith(x.name));
			if (num != -1)
			{
				text = localisationFiles[num];
			}
		}
		else
		{
			text = Resources.Load(path, typeof(TextAsset)) as TextAsset;
		}
		LoadTSV(text);
		return true;
	}

	private static bool LoadTSV(TextAsset text)
	{
		TVSReader tVSReader = new TVSReader(text.bytes);
		string[] array = tVSReader.ReadRow();
		if (array.Length < 2)
		{
			return false;
		}
		m_Languages = new string[array.Length - 1];
		for (int i = 0; i < m_Languages.Length; i++)
		{
			m_Languages[i] = array[i + 1];
		}
		string[] array2 = null;
		do
		{
			array2 = tVSReader.ReadRow();
			if (array2 == null || array2.Length <= 0 || !(array2[0] != string.Empty))
			{
				continue;
			}
			if (array2[0].ToLowerInvariant() != "end")
			{
				for (int j = 1; j < array2.Length; j++)
				{
					if (array2[j] != null && array2[j].Length > 2)
					{
						array2[j] = array2[j].Replace("\"", "\b");
						array2[j] = array2[j].Replace("\b\b", "\"");
						array2[j] = array2[j].Replace("\b", string.Empty);
					}
				}
				if (!m_stringTable.ContainsKey(array2[0]))
				{
					CheckForMarkup(ref array2);
					m_stringTable.Add(array2[0], array2);
				}
			}
			else
			{
				array2 = null;
			}
		}
		while (array2 != null);
		return true;
	}

	public static bool Get(string tag, out string localised, params LocToken[] findReplace)
	{
		if (tag.StartsWith("\"") && tag.EndsWith("\"") && tag.Length >= 2)
		{
			localised = tag.Substring(1, tag.Length - 2);
			return true;
		}
		if (m_stringTable.ContainsKey(tag))
		{
			SupportedLanguages language = GetLanguage();
			int num = (int)(language + 1);
			string[] array = m_stringTable[tag] as string[];
			string text = array[num];
			if (text.Length > 0)
			{
				if (findReplace.Length > 0)
				{
					StringBuilder stringBuilder = new StringBuilder(text);
					for (int i = 0; i < findReplace.Length; i++)
					{
						int num2 = -1;
						do
						{
							num2 = stringBuilder.ToString().IndexOf(findReplace[i].m_token, StringComparison.OrdinalIgnoreCase);
							if (num2 > -1)
							{
								stringBuilder.Remove(num2, findReplace[i].m_token.Length);
								stringBuilder.Insert(num2, findReplace[i].m_value);
							}
						}
						while (num2 != -1);
					}
					text = stringBuilder.ToString();
				}
				localised = text;
				return true;
			}
		}
		localised = "MT[" + tag + "]";
		return false;
	}

	public static string Get(string tag, params LocToken[] findReplace)
	{
		string localised = null;
		Get(tag, out localised, findReplace);
		return localised;
	}

	public static SupportedLanguages GetLanguage()
	{
		// patch
		Debugger debugger = GameObject.FindObjectOfType<Debugger>();
		if (debugger != null)
		{
			return debugger.debugLanguage;
		}
		// patch

		int num = -1;
		if (Application.isPlaying && SteamPlayerManager.Initialized)
		{
			num = SteamApps.GetCurrentGameLanguage().GetHashCode();
		}
		if (num == m_FrenchHashCode)
		{
			return SupportedLanguages.French;
		}
		if (num == m_ItalianHashCode)
		{
			return SupportedLanguages.Italian;
		}
		if (num == m_GermanHashCode)
		{
			return SupportedLanguages.German;
		}
		if (num == m_SpanishHashCode)
		{
			return SupportedLanguages.Spanish;
		}
		if (num == m_RussianHashCode)
		{
			return SupportedLanguages.Russian;
		}
		if (num == m_SChineseHashCode)
		{
			return SupportedLanguages.Chinese;
		}
		if (num == m_TChineseHashCode)
		{
			return SupportedLanguages.ChineseTraditional;
		}
		if (num == m_JapaneseHashCode)
		{
			return SupportedLanguages.Japanese;
		}
		if (num == m_KoreanHashCode)
		{
			return SupportedLanguages.Korean;
		}
		if (num == m_PolishHashCode)
		{
			return SupportedLanguages.Polish;
		}
		if (num == m_BrazilianHashCode)
		{
			return SupportedLanguages.Brazilian;
		}
		if (num == -1)
		{
			switch (Application.systemLanguage)
			{
			case SystemLanguage.French:
				return SupportedLanguages.French;
			case SystemLanguage.Italian:
				return SupportedLanguages.Italian;
			case SystemLanguage.German:
				return SupportedLanguages.German;
			case SystemLanguage.Spanish:
				return SupportedLanguages.Spanish;
			case SystemLanguage.Russian:
				return SupportedLanguages.Russian;
			case SystemLanguage.ChineseSimplified:
				return SupportedLanguages.Chinese;
			case SystemLanguage.ChineseTraditional:
				return SupportedLanguages.ChineseTraditional;
			case SystemLanguage.Japanese:
				return SupportedLanguages.Japanese;
			case SystemLanguage.Korean:
				return SupportedLanguages.Korean;
			case SystemLanguage.Polish:
				return SupportedLanguages.Polish;
			case SystemLanguage.Portuguese:
				return SupportedLanguages.Brazilian;
			default:
				return SupportedLanguages.English;
			}
		}
		return SupportedLanguages.English;
	}

	public static bool AreLanguagesLoaded()
	{
		return m_bIsLoaded;
	}

	private static void CheckForMarkup(ref string[] trans)
	{
		for (int i = 1; i < trans.Length; i++)
		{
			string text = string.Empty;
			int num = 0;
			bool flag = false;
			int num2 = -1;
			int num3 = -1;
			while (num < trans[i].Length)
			{
				num2 = trans[i].IndexOf('[', num);
				if (num2 > -1)
				{
					text += trans[i].Substring(num, num2 - num);
					num = num2 + 1;
					num3 = trans[i].IndexOf(']', num);
					if (num3 > -1)
					{
						switch (trans[i].Substring(num, num3 - num))
						{
						case "A":
						{
							flag = true;
							string text2 = "<color=red>\u0080</color>";
							text += text2;
							break;
						}
						case "B":
						{
							flag = true;
							string text2 = "<color=red>\u0081</color>";
							text += text2;
							break;
						}
						case "X":
						{
							flag = true;
							string text2 = "<color=red>\u0082</color>";
							text += text2;
							break;
						}
						case "Y":
						{
							flag = true;
							string text2 = "<color=red>\u0082</color>";
							text += text2;
							break;
						}
						}
						num = num3 + 1;
					}
					continue;
				}
				if (flag)
				{
					text += trans[i].Substring(num, trans[i].Length - num);
				}
				break;
			}
			if (flag)
			{
				trans[i] = text;
			}
		}
	}
}
