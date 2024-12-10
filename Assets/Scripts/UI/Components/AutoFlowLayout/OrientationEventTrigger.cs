using System;
using OpaGames.Blackcopter.UI.AutoFlowLayout;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OrientationEventTrigger : MonoBehaviour
{
    public UnityEvent OnLandscape, OnPortrait;

    private void Awake()
    {
        AutoFlowLayout.OnLandscape += () => OnLandscape?.Invoke();
        AutoFlowLayout.OnPortrait += () => OnPortrait?.Invoke();
    }
}
