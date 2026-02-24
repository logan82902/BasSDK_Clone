namespace ThunderRoad
{
    public class SurvivalEndlessModeOption : OptionEnum<LevelModuleSurvival.SurvivalModeLoopBehavior>
    {
        public SurvivalEndlessModeOption()
        {
            name = LevelOption.SurvivalWaves.Get();
            displayName = "Loop waves";
            description = "Loop waves instead of ending";
            currentValue = LevelModuleSurvival.SurvivalModeLoopBehavior.Loop;
            currentIntValue = (int)currentValue;
        }

    }
}
