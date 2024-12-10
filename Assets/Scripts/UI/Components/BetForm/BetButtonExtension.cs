using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BetButtonExtension : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Button button;
    [Space,SerializeField] private Sprite betSprite;
    [SerializeField] private SpriteState stateBet;

    [Space]
    [SerializeField] private Sprite cashoutSprite;
    [SerializeField] private SpriteState stateCashout;

    private void Awake()
    {
        if (!buttonImage) buttonImage = GetComponent<Image>();
        if (!button) button = GetComponent<Button>();
    }

    public void SetBetState() => SetState(betSprite, stateBet);

    public void SetCashoutState()=> SetState(cashoutSprite, stateCashout);

    private void SetState(Sprite defaultImage, SpriteState state)
    {
        buttonImage.sprite = defaultImage;
        button.spriteState = state;
    }
}
