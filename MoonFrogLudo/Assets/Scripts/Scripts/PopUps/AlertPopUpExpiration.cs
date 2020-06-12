using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AlertPopUpExpiration : MonoBehaviour
{
    public static AlertPopUpExpiration Inst;

    public AlertPopup[] alertPopups;

    private AlertPopup alert;

    void Awake()
    {
        Inst = this;
    }

    internal void ShowUpgradePopUp(string title, string msg, string[] options, UnityAction[] actions)
    {

        Debug.Log("Alert Upgradation");
        alert = (AlertPopup)Instantiate(alertPopups[options.Length], this.transform, false);

        Debug.Log("Alert Upgradation" + alert.name);
        alert.title.text = title;
        alert.msg.text = msg;
        alert.transform.SetAsLastSibling();
        for (int i = 0; i < options.Length; i++)
        {
            alert.buttonsLbls[i].text = options[i];
            alert.buttons[i].onClick.RemoveAllListeners();
            alert.buttons[i].onClick.AddListener(actions[i]);
            alert.buttons[i].onClick.AddListener(HidePopUp);
        }
    }

    internal void Show(string title, string msg, string[] options, UnityAction[] actions)
    {

        Debug.Log("Alert Upgradation"+UIManager.Inst.ParentRect.name);
        alert = (AlertPopup)Instantiate(alertPopups[options.Length], UIManager.Inst.ParentRect, false);

        Debug.Log("Alert Upgradation" + alert.name);
        alert.title.text = title;
        alert.msg.text = msg;
        alert.transform.SetAsLastSibling();
        for (int i = 0; i < options.Length; i++)
        {
            alert.buttonsLbls[i].text = options[i];
            alert.buttons[i].onClick.RemoveAllListeners();
            alert.buttons[i].onClick.AddListener(actions[i]);
            alert.buttons[i].onClick.AddListener(HidePopUp);
        }
    }

    internal void CustomPopup(string title, string msg, string[] options, UnityAction[] actions, int Index)
    {
        alert = (AlertPopup)Instantiate(alertPopups[Index], UIManager.Inst.ParentRect, false);
        alert.title.text = title;
        alert.msg.text = msg;
        alert.transform.SetAsLastSibling();
        for (int i = 0; i < options.Length; i++)
        {
            alert.buttonsLbls[i].text = options[i];
            alert.buttons[i].onClick.RemoveAllListeners();
            alert.buttons[i].onClick.AddListener(actions[i]);
            alert.buttons[i].onClick.AddListener(HidePopUp);
        }
    }

    internal void Show(string title, string msg)
    {
        alert = (AlertPopup)Instantiate(alertPopups[0], UIManager.Inst.ParentRect, false);
        alert.title.text = title;
        alert.msg.text = msg;
        alert.transform.SetAsLastSibling();
    }

    void HidePopUp()
    {
        if (alert != null)
        {
            Destroy(alert.gameObject);
            alert = null;
            //          iTween.ScaleTo (alert.gameObject,iTween.Hash ("scale",Vector3.zero,"time",0.3f,"easetype",iTween.EaseType.easeOutExpo,"oncomplete","DestroyME","oncompletetarget",this.gameObject));
        }
    }




    //  void DestoryME(){
    //      Destroy (this.gameObject);
    //  }

    //    public void OnButtonClick(string arg)
    //    {
    //
    //        switch(arg)
    //        {
    //            case "Ok":
    //                break;
    //            case "Cancel":
    //                break;
    //        }
    //
    //      HidePopUp ();
    //    }
}
