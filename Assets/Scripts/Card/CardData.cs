using UnityEngine;
using System.Collections.Generic; // 필요할 경우를 대비하여 추가

[CreateAssetMenu(menuName = "GameSettings/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    public enum CardSpecies
    {
        Beast,          //짐승   : 공격시 자신의 체력 1 회복
        Undead,         //언데드 : 공격시 자신의 체력 1 감소
        Machine,        //기계   : 받는 데미지 1 감소
        Savage,         //야수 (고블린,오크 류) : 사망시 코스트 + 1
        Dragon,         //드래곤 : 자기 앞에 있는 적은 공격 불가
        Zombie,         //좀비   : 처음 죽을시 체력 1로 부활
        Vampire,        //뱀파이어 : 공격한 수치만큼 체력을 증가시킴
        Unknown         //미상
    }

    // --- 카드 능력 타입 Enum  ---
    public enum CardAbilityType
    {
        Killer,      //살인마 : 공격시 적 카드 사망
        Mover,       //이동   : 공격 후 오른쪽 이동. 이동 불가시 왼쪽 이동
        Defender,    //수비   : 적의 공수 공격을 막음
        Flyer,       //공수   : 상대편 카드를 무시하고 적 플레이어 공격
        Diver,       //잠수   : 상대편 카드의 공격을 무시함
        GoblinRoad,  //고블린 로드 : 카드 소환시 양 옆에 고블린 카드 소환
        Tomb,        //무덤   : 카드 소환시 양 옆에 해골카드 소환
        Lifesteal,   //체력 흡수 : 공격한 데미지만큼 체력 회복
        Deathrattle, //죽음의 메아리 : 사망시 상대방 카드 10데미지
        Revenger,    //복수자 : 공격 받을시 상대방에게 1데미지
        Poisoner,    //독살자 : 공격한 적 2턴간 1뎀 + 공격 1 감소
        Weaker,      //나약한자 : 공격력이 턴당 2씩 감소함. (최소 1)
        None,        //없음
        
    }

    [Header("------- 카드 스탯 목록 ---------")]
    [SerializeField] string _cardName;
    [SerializeField] CardSpecies _species;
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] Sprite _cardImage;
    [SerializeField] int _cost;
    [SerializeField] Sprite _cardSkillImage;

    [Header("------- 카드 능력 ---------")] // 능력 관련 필드 추가
    [SerializeField] CardAbilityType _abilityType = CardAbilityType.None; // 기본값은 능력 없음
    [SerializeField] int _abilityValue; // 능력에 따라 필요한 추가 값 (예: 데미지 양, 회복량, 드로우 수 등)
    [SerializeField] string _abilityDescription; // 능력에 대한 설명 텍스트 (UI 표시용)

    public string CardName => _cardName;
    public CardSpecies Species => _species;
    public int Attack => _attack;
    public int Health => _health;
    public Sprite CardImage => _cardImage;
    public int Cost => _cost;
    public Sprite CardSkillImage => _cardSkillImage;

    // --- 능력 관련 프로퍼티 추가 ---
    public CardAbilityType AbilityType => _abilityType;
    public int AbilityValue => _abilityValue;
    public string AbilityDescription => _abilityDescription;
}