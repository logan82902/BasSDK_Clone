namespace ThunderRoad
{
    public class SurvivalWeaponRestrictionOption : OptionEnumFlag<LevelModuleSurvival.EquipmentSettings>
    {
        public SurvivalWeaponRestrictionOption() : base()
        {
            name = LevelOption.SurvivalRestriction.Get();
            displayName = "Progression Mode";
            description = "Keep or despawn reward items/skills on wave end";
            defaultValue = false;
        }
    }
}
