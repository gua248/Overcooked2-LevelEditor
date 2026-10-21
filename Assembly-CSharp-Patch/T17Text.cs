using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[AddComponentMenu("T17_UI/T17Text", 29)]
public class T17Text : Text, IT17EventHelper
{
	[Serializable]
	public class ImageData
	{
		[SerializeField]
		[ReadOnly]
		public string m_OriginalMarkup;

		[SerializeField]
		[Tooltip("This can be changed in editor BUT will be changed to the correct image at runtime!")]
		public Sprite IconSprite;

		[SerializeField]
		[ReadOnly]
		public Vector2 Position = Vector2.zero;

		[SerializeField]
		[ReadOnly]
		public bool OutOfBounds;

		[SerializeField]
		public Vector2 Offset = Vector2.zero;

		[SerializeField]
		public Vector2 AlignOffset = new Vector2(0f, 0.2f);

		[SerializeField]
		public int Size = 34;

		[SerializeField]
		[HideInInspector]
		public T17Image ImageObj;

		[SerializeField]
		[HideInInspector]
		public int OriginalQuadIndex;

		[SerializeField]
		[HideInInspector]
		public int PlaceInString;

		[SerializeField]
		[HideInInspector]
		public bool NeedsToBeDeleted;
	}

	public bool m_bNeedsLocalization = true;

	public string m_LocalizationTag = "XXX.YYY.ZZZ";

	public string m_PlaceholderText = string.Empty;

	public string m_NonLocalizedKeyboardText = string.Empty;

	public bool m_bTagFound;

	public LocToken[] m_Replacements;

	private static ThaiFontProcessor s_ThaiFontProcessor = new ThaiFontProcessor();

	public Func<bool> m_ReleaseOnPointerClickDelegate;

	private string m_MarkedUpString = string.Empty;

	private PlayerManager m_PlayerManager;

	private List<ITextProcessor> m_TextProcessors;

	public override Color color
	{
		get
		{
			return base.color;
		}
		set
		{
			base.color = value;
		}
	}

	protected override void Start()
	{
		base.Start();
		if (Application.isPlaying)
		{
			m_PlayerManager = GameUtils.RequireManager<PlayerManager>();
		}
		m_TextProcessors = new List<ITextProcessor>(GetComponents<ITextProcessor>());
		if (material == null)
		{
			material = base.font.material;
		}
		if (m_bNeedsLocalization)
		{
			Convert();
		}
		else if (!m_bTagFound && !string.IsNullOrEmpty(m_NonLocalizedKeyboardText) && ShouldUseKeyboardText())
		{
			CheckMarkup(m_NonLocalizedKeyboardText);
			Apply(m_NonLocalizedKeyboardText);
		}
		else
		{
			CheckMarkup(text);
			Apply(text);
		}
	}

	public void Convert()
	{
		if (string.IsNullOrEmpty(m_PlaceholderText))
		{
			m_PlaceholderText = text;
		}
		Localization.LoadDictionarys(false);
		string localised = string.Empty;
		m_bTagFound = false;
		if (ShouldUseKeyboardText())
		{
			if (m_Replacements == null)
			{
				m_bTagFound = Localization.Get(m_LocalizationTag + "_Keyboard", out localised);
			}
			else
			{
				m_bTagFound = Localization.Get(m_LocalizationTag + "_Keyboard", out localised, m_Replacements);
			}
		}
		if (!m_bTagFound)
		{
			if (m_Replacements == null)
			{
				m_bTagFound = Localization.Get(m_LocalizationTag, out localised);
			}
			else
			{
				m_bTagFound = Localization.Get(m_LocalizationTag, out localised, m_Replacements);
			}
		}
		if (m_bTagFound)
		{
			CheckMarkup(localised);
			text = localised;
		}
		else
		{
			CheckMarkup(localised);
			text = localised;
		}
		Apply(text);
	}

	public void CheckMarkup()
	{
		if (!m_bTagFound && !string.IsNullOrEmpty(m_NonLocalizedKeyboardText) && ShouldUseKeyboardText())
		{
			CheckMarkup(m_NonLocalizedKeyboardText);
		}
		else
		{
			CheckMarkup(text);
		}
	}

	private void CheckMarkup(string text)
	{
		if (!(this == null))
		{
			m_MarkedUpString = text;
		}
	}

	private void Apply(string str)
	{
		if (m_TextProcessors != null)
		{
			for (int i = 0; i < m_TextProcessors.Count; i++)
			{
				m_TextProcessors[i].ProcessText(ref str);
			}
		}
		m_MarkedUpString = str;
		text = str;
	}

	public void SetLocalisedTextCatchAll(string text)
	{
		bool flag = false;
		if ((!m_bNeedsLocalization) ? (this.text != text) : (m_LocalizationTag != text))
		{
			this.text = text;
			SetNewPlaceHolder(text);
			m_bNeedsLocalization = true;
			SetNewLocalizationTag(text);
		}
	}

	public void SetNewLocalizationTag(string newTag)
	{
		// patch
		newTag = LocalizationParser.ParseText(newTag);
		// patch
		if (m_bNeedsLocalization)
		{
			m_LocalizationTag = newTag;
			Convert();
		}
		else
		{
			m_LocalizationTag = string.Empty;
			CheckMarkup(newTag);
			Apply(newTag);
		}
	}

	public void SetNewPlaceHolder(string newPlaceholder)
	{
		m_PlaceholderText = newPlaceholder;
	}

	public void SetNonLocalizedText(string text)
	{
		ResetLocalisation();
		this.text = text;
		CheckMarkup();
		Apply(this.text);
	}

	private void ResetLocalisation()
	{
		m_bNeedsLocalization = false;
		m_LocalizationTag = string.Empty;
		m_PlaceholderText = string.Empty;
	}

	private void LanguageChanged()
	{
		if (m_bNeedsLocalization)
		{
			Convert();
		}
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		bool flag = false;
		if (m_TextProcessors != null)
		{
			for (int i = 0; i < m_TextProcessors.Count; i++)
			{
				if (m_TextProcessors[i] != null && m_TextProcessors[i].HasEmbeddedImages(this.text))
				{
					flag = true;
				}
			}
		}
		if (flag)
		{
			string text = m_Text;
			m_Text = m_MarkedUpString;
			base.OnPopulateMesh(toFill);
			m_Text = text;
			if (!Application.isPlaying || m_TextProcessors == null)
			{
				return;
			}
			for (int j = 0; j < m_TextProcessors.Count; j++)
			{
				if (m_TextProcessors[j] != null)
				{
					m_TextProcessors[j].OnPopulateMesh(toFill);
				}
			}
		}
		else
		{
			base.OnPopulateMesh(toFill);
		}
	}

	private bool ShouldUseKeyboardText()
	{
		if (m_PlayerManager != null)
		{
			GamepadUser user = m_PlayerManager.GetUser(EngagementSlot.One);
			return user != null && user.ControlType == GamepadUser.ControlTypeEnum.Keyboard;
		}
		return false;
	}

	public T17EventSystem GetDomain()
	{
		return null;
	}

	public GameObject GetGameobject()
	{
		return base.gameObject;
	}

	public void SetEventSystem(T17EventSystem gamersEventSystem = null)
	{
		if (!m_bTagFound && !string.IsNullOrEmpty(m_NonLocalizedKeyboardText) && ShouldUseKeyboardText())
		{
			CheckMarkup(m_NonLocalizedKeyboardText);
			Apply(m_NonLocalizedKeyboardText);
		}
		else
		{
			CheckMarkup(text);
			Apply(text);
		}
	}
}
