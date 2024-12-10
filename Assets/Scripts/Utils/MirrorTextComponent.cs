using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class MirrorTextComponent : MonoBehaviour
{
    [SerializeField] private TMP_Text targetText;
    private TMP_Text myText;

    private void OnEnable()
    {
        if (!myText) myText = GetComponent<TMP_Text>();
        myText.text = targetText.text;
    }
}
