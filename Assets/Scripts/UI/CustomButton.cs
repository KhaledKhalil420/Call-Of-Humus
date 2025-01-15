using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class CustomButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color selectedColor;

    private Vector3 startingSize, currentSize;
    private Vector3 startingPosition, offsetedPosition, currentPosition;
    public Vector3 offset;
    private Color startingColor, currentColor;

    private Image img;
    private TMP_Text text;

    private RectTransform rectTransform;
    
    public UnityEvent Event;
    private bool IsMouseOver;

    public bool selectable;
    
    public enum CustomAnimation {scale, line}
    public CustomAnimation customAnimation = CustomAnimation.scale;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startingSize = transform.localScale;
        currentSize = startingSize;

        text = GetComponentInChildren<TMP_Text>();

        if(img != null)
        {
            img = GetComponentInChildren<Image>();
            startingColor = img.color;
            currentColor = startingColor;
        }

        else if(text != null)
        {
            text = GetComponentInChildren<TMP_Text>();
            startingColor = text.color;
            currentColor = startingColor;
        }

        startingPosition = rectTransform.localPosition;
        offsetedPosition = rectTransform.localPosition + offset;

        currentPosition = startingPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;

        if(UiManager.instance != null)
        {
            if(selectable)
                UiManager.instance.currentSelectedObject = gameObject;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
        
        if(UiManager.instance != null)
        {
            if(selectable & UiManager.instance.currentSelectedObject == gameObject)
                 UiManager.instance.currentSelectedObject = null;
        }
    }
    
    private void OnEnable()
    {
        IsMouseOver = false;
    }

    private void OnDisable()
    {
        if(UiManager.instance != null)
        {
            if(selectable & UiManager.instance.currentSelectedObject == gameObject)
                UiManager.instance.currentSelectedObject = null;
        }

        currentPosition = startingPosition;
    }

    private void Update()
    {
        if(IsMouseOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Event?.Invoke();
            }

            currentSize = startingSize * 1.3f;
            currentColor = selectedColor;

            currentPosition = offsetedPosition;
        }

        else
        {
            currentSize = startingSize;
            currentColor = startingColor;

            currentPosition = startingPosition;
        }


        switch(customAnimation)
        {
            //SCALE ANIMATION
            case CustomAnimation.scale:

            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, currentSize, 6 * Time.unscaledDeltaTime);
            
            if(img != null)
            img.color = Color.Lerp(img.color, currentColor, 6 * Time.unscaledDeltaTime);

            if(text != null)
            text.color = Color.Lerp(text.color, currentColor, 6 * Time.unscaledDeltaTime);
            break;
            
            //LINE ANIMATION
            case CustomAnimation.line:
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, currentPosition, 6 * Time.unscaledDeltaTime);
            break;
        }
    }

    public void OpenUrl(string url)
    {
        Application.OpenURL(url);
    }
}