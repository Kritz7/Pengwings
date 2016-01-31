using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class barthing : MonoBehaviour {

	public static barthing mainBar;
	public List<Text> allText;

	public RectTransform baseBarRect;
	public HorizontalLayoutGroup layoutGroup;

	void Start()
	{
		mainBar = this;
		SetBarPercentage(0.5f);

		foreach(Text text in allText)
		{
			text.text = "";
		}
	}

	public void SetBarPercentage(float percentage)
	{
		Debug.Log("yo!");

		baseBarRect.gameObject.SetActive(false);
		layoutGroup.padding.left = Mathf.RoundToInt(Mathf.Lerp(0, baseBarRect.rect.width, percentage));

		baseBarRect.gameObject.SetActive(true);

		Debug.Log(percentage);

		if(percentage > 1)
		{
			foreach(Text text in allText)
			{
				text.text = "Yellow player wins!";
				StartCoroutine(restart());
			}
		}
		if(percentage < 0)
		{
			foreach(Text text in allText)
			{
				text.text = "Bleu player wins!";
				StartCoroutine(restart());
			}
		}
	}

	IEnumerator restart()
	{
		yield return new WaitForSeconds(2f);

		Application.LoadLevel(0);
	}
}
