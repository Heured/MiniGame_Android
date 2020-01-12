
/*
public static class Screen
{
    // 窗口大小
    public static float DefinedScreenWidth = 1536;
    public static float DefinedScreenHeight = 2048;
}
*/
// 一系列命名
public static class StickHeroGameSceneChildName
{
    public static string HeroName = "hero";
    public static string StickName = "stick";
    public static string StackName = "stack";
    public static string StackMidName = "stack_mid";
    public static string ScoreName = "score";
    public static string TopScoreName = "top_score";
    public static string TipName = "tip";
    public static string PerfectName = "perfect";
    public static string GameOverLayerName = "over";
    public static string RetryButtonName = "retry";
    public static string StartButtonName = "start";
    public static string SoundButtonName = "quiet";
    public static string LeaderBoardButtonName = "leaderboard";
    public static string BackButtonName = "back";
    public static string HighScoreName = "highscore";
}

public static class StickHeroGameSceneActionKey
{
    public static string WalkAction = "walk";
    public static string StickGrowAudioAction = "stick_grow_audio";
    public static string StickGrowAction = "stick_grow";
    public static string HeroScaleAction = "hero_scale";
}

//一系列音频命名
public static class StickHeroGameSceneEffectAudioName
{
    public static string DeadAudioName = "dead";
    public static string StickGrowAudioName = "stick_grow_loop";
    public static string StickGrowOverAudioName = "kick";
    public static string StickFallAudioName = "fall";
    public static string StickTouchMidAudioName = "touch_mid";
    public static string VictoryAudioName = "victory";
    public static string HighScoreAudioName = "highScore";
    public static string QuietAudioName = "quiet";
}

//所有对象的z轴位置，数值越大越靠前
public enum StickHeroGameSceneZposition
{
     backgroundZposition = 60,
     stackZposition = 50,
     stackMidZposition = 40,
     stickZposition = 30,
     scoreBackgroundZposition = 20,
     heroZposition, scoreZposition, tipZposition, perfectZposition = 10,
     emitterZposition = 9,
     gameOverZposition = 8,
}
