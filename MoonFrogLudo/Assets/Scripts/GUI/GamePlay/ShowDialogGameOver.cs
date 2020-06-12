using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShowDialogGameOver : MonoBehaviour
{

    public delegate void OnOKClicked();
    OnOKClicked onOKCallBack;
    [SerializeField] TMP_Text GameOverTitleText;
    [SerializeField] TMP_Text GameOverMSGText;

    public void ShowGameOverDialog(string MSGText, OnOKClicked onOKClick)
    {
        GameOverMSGText.text= MSGText;
        onOKCallBack = onOKClick;
    }
   public void OnOK()
    {
        if (onOKCallBack != null)
        {
            onOKCallBack();
            gameObject.SetActive(false);
        }
    }
}
