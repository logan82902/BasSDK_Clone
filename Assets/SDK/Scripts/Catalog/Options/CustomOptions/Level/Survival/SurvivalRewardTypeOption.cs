namespace ThunderRoad
{
    public class SurvivalRewardTypeOption : OptionEnumFlag<LevelModuleSurvival.RewardTypes>
    {
        public SurvivalRewardTypeOption() : base()
        {
            name = LevelOption.SurvivalRewards.Get();
            displayName = "Reward Type Active";
            description = "Reward type can spawn or not";
            defaultValue = false;
        }
    }
}
