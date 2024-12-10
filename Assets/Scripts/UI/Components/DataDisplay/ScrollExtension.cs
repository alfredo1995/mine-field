using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollExtension : MonoBehaviour
{
    [SerializeField, Range(.5f, 1)] private float edgeTreshold = 1;
    private ScrollRect scroll;
    private Vector2 startPos;
    private Vector2 endPos;
    private bool? atStart;

    public UnityEvent OnReachStart, OnReachEnd;
    
    private void Awake()
    {
        scroll = GetComponent<ScrollRect>();
        CalculateArea();
        scroll.onValueChanged.AddListener(CheckScroll);
    }

    private void CalculateArea()
    {
        var size = scroll.content.rect.size;
        startPos = Vector2.zero;
        var pivot = scroll.content.pivot;
        startPos.x -= size.x * pivot.x;
        startPos.y -= size.y * pivot.y;
        
        endPos = Vector2.zero;
        endPos.x += size.x * pivot.x;
        endPos.y += size.y * pivot.y;
    }

    public void ScrollToStartVertical()
    {
        CalculateArea();
        StartCoroutine(ScrollTo(
            new Vector2(scroll.content.anchoredPosition.x, startPos.y)));
    }

    public void ScrollToEndVertical()
    {
        CalculateArea();
        StartCoroutine(ScrollTo(new(scroll.content.anchoredPosition.x, endPos.y)));
    }
    
    public void ScrollToVertical(float value)
    {
        CalculateArea();
        StartCoroutine(ScrollTo(
            new Vector2(scroll.content.anchoredPosition.x, value)));
    }

    private IEnumerator ScrollTo(Vector2 target)
    {
        float speed = scroll.scrollSensitivity;
        Vector2 currentPos = scroll.content.anchoredPosition;
        int iterations = Mathf.CeilToInt(Vector2.Distance(target, currentPos) / speed);
        Vector2 ticks = Vector2.zero;
        ticks.x = (target.x - currentPos.x)/speed;
        ticks.y = (target.y - currentPos.y)/speed;
        for (int i = 0; i < iterations; i++)
        {
            scroll.content.anchoredPosition += ticks;
            yield return null;
        }

    }
    
    //Checks

    private void CheckScroll(Vector2 value)
    {
        Vector2 start = Vector2.zero;
        Vector2 end = Vector2.zero;
        if (scroll.horizontal)
        {
            start.x = edgeTreshold;
            end.x = 1 - edgeTreshold;
        }
        if (scroll.vertical)
        {
            start.y = edgeTreshold;
            end.y = 1 - edgeTreshold;
        }
        
        if (value.x >= start.x && value.y >= start.y)
        {
            //Debug.Log($"{start} | {atStart}");
            if (atStart != true)
            {
                //Debug.Log("Reached Start "+value);
                atStart = true;
                OnReachStart?.Invoke();
            }
            return;
        }
        if (value.x <= end.x && value.y <= end.y)
        {
            if (atStart != false)
            {
                //Debug.Log("Reached End "+value);
                atStart = false;
                OnReachEnd?.Invoke();
            }
            return;
        }
        //Debug.Log($"reseting at: {value} - {Time.time}");
        atStart = null;
    }
}
