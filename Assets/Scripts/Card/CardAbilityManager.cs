using UnityEngine;
/// <summary>
/// <para>카드 소환 직후 해당 카드의 종족 능력(Species Ability)과 스킬 능력(Skill Ability)에 해당하는</para>
/// <para>IAbility 구현체들을 동적으로 생성하고 초기화하여 FieldCard에 등록하는 역할을 담당합니다.</para>
/// <para>이 스크립트는 **팩토리 패턴(Factory Pattern)**과 **다형성(Polymorphism)**을 활용하여</para>
/// <para>다양한 카드 능력들을 유연하고 확장성 있게 관리합니다.</para>
/// </summary>
public class CardAbilityManager : MonoBehaviour
{
    // 싱글톤 패턴을 적용하여 GameManager 등 다른 스크립트에서 쉽게 접근할 수 있도록 합니다.
    public static CardAbilityManager Instance { get; private set; }

    /// <summary>
    /// 싱글톤 인스턴스를 초기화합니다.
    /// 이미 인스턴스가 존재하면 현재 오브젝트를 파괴하여 중복 생성을 방지합니다.
    /// </summary>
    private void Awake()
    {
        Instance = this;


        //if (Instance != null && Instance != this)
        //{
        //    Destroy(this.gameObject); // 이미 인스턴스가 있다면 현재 게임 오브젝트 파괴
        //}
        //else
        //{
        //    Instance = this;                    // 현재 인스턴스를 싱글톤으로 설정
        //    DontDestroyOnLoad(this.gameObject); // 씬 전환 시 파괴되지 않도록 설정 (선택 사항, 필요에 따라)
        //}
    }

    /// <summary>
    /// <para>새로 필드에 소환된 FieldCard 오브젝트에 해당 CardData에 정의된 모든 능력들을 등록합니다.</para>
    /// <para>이 메서드는 카드 소환 로직(예: CardSpawner, FieldManager 등)에서 호출됩니다.</para>
    /// </summary>
    /// <param name="card">능력을 등록할 FieldCard 컴포넌트</param>
    /// <param name="data">능력 정보를 포함하는 CardData ScriptableObject</param>
    public void RegisterAll(FieldCard card, CardData data)
    {
        // 1) 종족(Species) 능력 등록
        // CardData.CardSpecies enum 값을 기반으로 적절한 종족 능력 구현체(IAbility)를 생성합니다.
        var speciesAb = CreateSpeciesAbility(data.Species);
        if (speciesAb != null)
        {
            // 생성된 능력 인스턴스를 초기화하고 FieldCard에 등록합니다.
            // FieldCard는 등록된 능력을 관리하며, 해당 능력은 게임 이벤트에 반응하게 됩니다.
            speciesAb.Initialize(card, data); // 능력 초기화 (예: 해당 능력을 소유한 카드 정보 설정)
            card.RegisterAbility(speciesAb);  // FieldCard에 능력 등록
            Debug.Log($"[CardAbilityManager] {card.name}에 종족 능력 '{data.Species}' 등록 완료.");
        }

        // 2) 스킬(AbilityType) 능력 등록
        // CardData.CardAbilityType enum 값을 기반으로 적절한 스킬 능력 구현체(IAbility)를 생성합니다.
        var skillAb = CreateSkillAbility(data.AbilityType);
        if (skillAb != null)
        {
            // 생성된 능력 인스턴스를 초기화하고 FieldCard에 등록합니다.
            skillAb.Initialize(card, data); // 능력 초기화
            card.RegisterAbility(skillAb);  // FieldCard에 능력 등록
            Debug.Log($"[CardAbilityManager] {card.name}에 스킬 능력 '{data.AbilityType}' 등록 완료.");
        }
    }

    /// <summary>
    /// <para>주어진 CardSpecies enum 값에 따라 해당 종족 능력의 IAbility 구현체를 생성합니다.</para>
    /// <para>**팩토리 메서드(Factory Method)** 패턴의 일환으로, 능력 객체 생성 로직을 캡슐화합니다.</para>
    /// </summary>
    /// <param name="s">생성할 종족 능력의 타입</param>
    /// <returns>생성된 IAbility 인스턴스 또는 해당 종족 능력이 없을 경우 null</returns>
    private IAbility CreateSpeciesAbility(CardData.CardSpecies s)
    {
        switch (s)
        {
            case CardData.CardSpecies.Beast: return new BeastSpeciesAbility();
            case CardData.CardSpecies.Undead: return new UndeadSpeciesAbility();
            case CardData.CardSpecies.Machine: return new MachineSpeciesAbility();
            case CardData.CardSpecies.Savage: return new SavageSpeciesAbility();
            case CardData.CardSpecies.Dragon: return new DragonSpeciesAbility();
            case CardData.CardSpecies.Zombie: return new ZombieSpeciesAbility();
            case CardData.CardSpecies.Vampire: return new VampireSpeciesAbility();
            // 새로운 종족 능력이 추가될 경우 여기에 case 문만 추가하면 되므로,
            // 기존 코드를 수정하지 않고도 확장성이 뛰어납니다 (개방-폐쇄 원칙 준수).
            default: return null; // 정의되지 않은 종족 능력인 경우
        }
    }

    /// <summary>
    /// <para>주어진 CardAbilityType enum 값에 따라 해당 스킬 능력의 IAbility 구현체를 생성합니다.</para>
    /// <para>**팩토리 메서드(Factory Method)** 패턴의 일환으로, 능력 객체 생성 로직을 캡슐화합니다.</para>
    /// </summary>
    /// <param name="t">생성할 스킬 능력의 타입</param>
    /// <returns>생성된 IAbility 인스턴스 또는 해당 스킬 능력이 없을 경우 null</returns>
    private IAbility CreateSkillAbility(CardData.CardAbilityType t)
    {
        switch (t)
        {
            case CardData.CardAbilityType.Lifesteal: return new LifestealAbility();
            case CardData.CardAbilityType.Deathrattle: return new DeathrattleAbility();
            case CardData.CardAbilityType.Revenger: return new RevengerAbility();
            case CardData.CardAbilityType.Defender: return new DefenderAbility();
            case CardData.CardAbilityType.Flyer: return new FlyerAbility();
            case CardData.CardAbilityType.Killer: return new KillerAbility();
            case CardData.CardAbilityType.Weaker: return new WeakerAbility();
            case CardData.CardAbilityType.GoblinRoad: return new GoblinRoadAbility();
            case CardData.CardAbilityType.Poisoner: return new PoisonerAbility();
            case CardData.CardAbilityType.Mover: return new MoverAbility();
            case CardData.CardAbilityType.Diver: return new DiverAbility();
            // 새로운 스킬 능력이 추가될 경우 여기에 case 문만 추가하면 되므로,
            // 기존 코드를 수정하지 않고도 확장성이 뛰어납니다 (개방-폐쇄 원칙 준수).
            // ... 추가 스킬 능력 ...
            default: return null; // 정의되지 않은 스킬 능력인 경우
        }
    }
}