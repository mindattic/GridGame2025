namespace Assets.Scripts.Models
{
    public enum TurnSelectionMode
    {
        FreeSelect = 0,            // Player can move any hero
        PreferActiveWithBonus = 1, // Player can select any hero; callers may grant a bonus if the active hero is chosen
        ActiveOnly = 2             // Player can only move the hero on the current timeline block
    }
}
