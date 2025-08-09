namespace Assets.Helper
{

    public static class Rarities
    {
        public static Rarity Junk = new Rarity("Junk", ColorHelper.RGB(128, 128, 128));
        public static Rarity Common = new Rarity("Common", ColorHelper.RGB(255, 255, 255));
        public static Rarity Uncommon = new Rarity("Uncommon", ColorHelper.RGB(30, 255, 0));
        public static Rarity Rare = new Rarity("Rare", ColorHelper.RGB(0, 112, 221));
        public static Rarity Epic = new Rarity("Epic", ColorHelper.RGB(163, 53, 238));
        public static Rarity Legendary = new Rarity("Legendary", ColorHelper.RGB(255, 128, 0));
    }

}