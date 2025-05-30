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
    [SerializeField] string _cardName;
    [SerializeField] CardSpecies _species;
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] Sprite _cardImage;

    public string CardName => _cardName;
    public CardSpecies Species => _species;
    public int Attack => _attack;
    public int Health => _health;
    public Sprite CardImage => _cardImage;
}