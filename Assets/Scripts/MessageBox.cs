using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class MessageBox : MonoBehaviour {
    [SerializeField] CanvasGroup messageBox;
    [SerializeField] TextMeshProUGUI tittleBoxText;
    [SerializeField] TextMeshProUGUI messageBoxText;
    [SerializeField] CustomButton yesButton;
    [SerializeField] CustomButton noButton;
    public float popupDuration = 0.5f;

    public static MessageBox instance = null;
    UnityAction defaultAction;

    private void Awake()
    {
        instance = this;
        defaultAction = () => { HideMessageBox(); };
    }

    GameObject lastSelectedGO;

    public void ShowMessageBox(string tittle, string message, bool autoHide = true, string yesCaption = "Yes", string noCaption = "No", UnityAction yesAction = null, UnityAction noAction = null)
    {
        yesButton._text.text = yesCaption;
        noButton._text.text = noCaption;
        tittleBoxText.text = tittle;
        messageBoxText.text = message;
        yesButton.onClick.RemoveAllListeners();
        noButton.gameObject.SetActive(true);
        noButton.onClick.RemoveAllListeners();
        if (yesAction != null)
            yesButton.onClick.AddListener(yesAction);
        if (noAction != null)
            noButton.onClick.AddListener(yesAction);
        if (autoHide)
        {
            yesButton.onClick.AddListener(defaultAction);
            noButton.onClick.AddListener(defaultAction);
        }
        messageBox.gameObject.SetActive(true);
        lastSelectedGO = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        messageBox.DOKill();
        messageBox.DOFade(1f, popupDuration);
    }

    public void ShowInfoBox(string tittle, string message, string okCaption = "Ok")
    {
        tittleBoxText.text = tittle;
        messageBoxText.text = message;
        yesButton._text.text = okCaption;
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(defaultAction);
        noButton.gameObject.SetActive(false);
        messageBox.gameObject.SetActive(true);
        lastSelectedGO = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
        messageBox.DOKill();
        messageBox.DOFade(1f, popupDuration);
    }

    public void HideMessageBox()
    {
        messageBox.DOKill();
        messageBox.DOFade(0f, popupDuration).OnComplete(() =>
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedGO);
            messageBox.gameObject.SetActive(false);
        });
    }
}
