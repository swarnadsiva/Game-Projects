using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public abstract class SelectableUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public bool IsSelected;
    public const float FADE_SPEED = 2f;

    public abstract void OnPointerEnter(PointerEventData eventData);

    public abstract void OnPointerExit(PointerEventData eventData);

}
