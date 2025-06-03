using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    public enum CardSpecies
    {
        Beast,
        Undead,
        Machine,
        Savage,
        Unknown
    }
    [Header("------- 카드 스탯 목록 ---------")]
    [SerializeField] string _cardName;
    [SerializeField] CardSpecies _species;
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] Sprite _cardImage;
    [SerializeField] int _cost;

    public string CardName => _cardName;
    public CardSpecies Species => _species;
    public int Attack => _attack;
    public int Health => _health;
    public Sprite CardImage => _cardImage;
    public int Cost => _cost;
}