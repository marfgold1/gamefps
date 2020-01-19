using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using TMPro;

public class CustomButton : Button {
    public Color selectedColor = Color.black;
    public Color unselectedColor = Color.white;
    public Image _image;
    public TextMeshProUGUI _text;
    public AudioClip hoverClip;

    public override void OnPointerEnter(PointerEventData ped)
    {
        if(interactable)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public override void OnPointerExit(PointerEventData eventData) { }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        SceneHelper.instance.audioSource.PlayOneShot(hoverClip);
        _image.DOKill();
        _image.DOFillAmount(1f, 0.3f);
        _text.DOKill();
        _text.DOColor(selectedColor, 0.3f);
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        _image.DOKill();
        _image.DOFillAmount(0f, 0.3f);
        _text.DOKill();
        _text.DOColor(unselectedColor, 0.3f);
    }
}
