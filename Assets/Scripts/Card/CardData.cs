using UnityEngine;
using System.Collections.Generic; // �ʿ��� ��츦 ����Ͽ� �߰�

[CreateAssetMenu(menuName = "GameSettings/CardData", fileName = "CardData")]
public class CardData : ScriptableObject
{
    public enum CardSpecies
    {
        Beast,          //����   : ���ݽ� �ڽ��� ü�� 1 ȸ��
        Undead,         //�𵥵� : ���ݽ� �ڽ��� ü�� 1 ����
        Machine,        //���   : �޴� ������ 1 ����
        Savage,         //�߼� (���,��ũ ��) : ����� �ڽ�Ʈ + 1
        Dragon,         //�巡�� : �ڱ� �տ� �ִ� ���� ���� �Ұ�
        Zombie,         //����   : ó�� ������ ü�� 1�� ��Ȱ
        Vampire,        //�����̾� : ������ ��ġ��ŭ ü���� ������Ŵ
        Unknown         //�̻�
    }

    // --- ī�� �ɷ� Ÿ�� Enum  ---
    public enum CardAbilityType
    {
        Killer,      //���θ� : ���ݽ� �� ī�� ���
        Mover,       //�̵�   : ���� �� ������ �̵�. �̵� �Ұ��� ���� �̵�
        Defender,    //����   : ���� ���� ������ ����
        Flyer,       //����   : ����� ī�带 �����ϰ� �� �÷��̾� ����
        Diver,       //���   : ����� ī���� ������ ������
        GoblinRoad,  //��� �ε� : ī�� ��ȯ�� �� ���� ��� ī�� ��ȯ
        Tomb,        //����   : ī�� ��ȯ�� �� ���� �ذ�ī�� ��ȯ
        Lifesteal,   //ü�� ��� : ������ ��������ŭ ü�� ȸ��
        Deathrattle, //������ �޾Ƹ� : ����� ���� ī�� 10������
        Revenger,    //������ : ���� ������ ���濡�� 1������
        Poisoner,    //������ : ������ �� 2�ϰ� 1�� + ���� 1 ����
        Weaker,      //�������� : ���ݷ��� �ϴ� 2�� ������. (�ּ� 1)
        None,        //����
        
    }

    [Header("------- ī�� ���� ��� ---------")]
    [SerializeField] string _cardName;
    [SerializeField] CardSpecies _species;
    [SerializeField] int _attack;
    [SerializeField] int _health;
    [SerializeField] Sprite _cardImage;
    [SerializeField] int _cost;
    [SerializeField] Sprite _cardSkillImage;

    [Header("------- ī�� �ɷ� ---------")] // �ɷ� ���� �ʵ� �߰�
    [SerializeField] CardAbilityType _abilityType = CardAbilityType.None; // �⺻���� �ɷ� ����
    [SerializeField] int _abilityValue; // �ɷ¿� ���� �ʿ��� �߰� �� (��: ������ ��, ȸ����, ��ο� �� ��)
    [SerializeField] string _abilityDescription; // �ɷ¿� ���� ���� �ؽ�Ʈ (UI ǥ�ÿ�)

    public string CardName => _cardName;
    public CardSpecies Species => _species;
    public int Attack => _attack;
    public int Health => _health;
    public Sprite CardImage => _cardImage;
    public int Cost => _cost;
    public Sprite CardSkillImage => _cardSkillImage;

    // --- �ɷ� ���� ������Ƽ �߰� ---
    public CardAbilityType AbilityType => _abilityType;
    public int AbilityValue => _abilityValue;
    public string AbilityDescription => _abilityDescription;
}