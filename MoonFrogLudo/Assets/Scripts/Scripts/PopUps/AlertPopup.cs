using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertPopup : MonoBehaviour {

	public List<Button> buttons;
	public List<TMP_Text> buttonsLbls;
	public TMP_Text title;
	public TMP_Text msg;

	[SerializeField]RectTransform popupRect;

	void Awake(){
		popupRect.localScale = Vector3.zero;
	}

	void Start(){
		//iTween.ScaleTo (popupRect.gameObject,iTween.Hash ("scale",Vector3.one,"time",0.7f,"easetype",iTween.EaseType.easeOutExpo));

		if (buttons.Count < 1) {
			Destroy (this.gameObject,3);
		}
	}
}
