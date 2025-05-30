using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;

    public TMP_Text NameText;
    public TMP_Text SpeciesText;
    public TMP_Text AttackText;
    public TMP_Text HealthText;
    public Image CardImage;

    void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        NameText.text = cardData.CardName;
        SpeciesText.text = cardData.Species.ToString();
        AttackText.text = cardData.Attack.ToString();
        HealthText.text = cardData.Health.ToString();
        CardImage.sprite = cardData.CardImage;
    }
}