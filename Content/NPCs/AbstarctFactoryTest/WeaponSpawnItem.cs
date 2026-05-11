using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.AbstarctFactoryTest
{
    public class WeaponSpawnItem : ModItem
    {
        public override string Texture => "OBJTest/Content/NPCs/AbstarctFactoryTest/WeaponSpawnItem";

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = 0;
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.Item1;
        }

        public override bool? UseItem(Player player)
        {
            WeaponSpawnHelper.SetFactory("Red");
            WeaponSpawnHelper.CreateSword();
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            WeaponSpawnHelper.SetFactory("Blue");
            WeaponSpawnHelper.CreateBow();
            return true;
        }
    }
}
