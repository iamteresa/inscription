// Assets/Scripts/GameSettings.cs
public static class GameSettings
{
    public enum Difficulty { Easy, Normal, Hard, Nightmare }

    // 현재 선택된 난이도
    public static Difficulty CurrentDifficulty { get; private set; } = Difficulty.Normal;

    /// <summary>
    /// 난이도를 설정합니다.
    /// </summary>
    public static void SetDifficulty(Difficulty diff)
    {
        CurrentDifficulty = diff;
    }
}
