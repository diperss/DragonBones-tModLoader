using Terraria;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.AbstarctFactoryTest
{
    public static class WeaponSpawnHelper
    {
        public static NPCFactory CurrentFactory { get; private set; } = new RedFactory();

        public static void CreateBow() => CurrentFactory.CreateBow();
        public static void CreateStuff() => CurrentFactory.CreateStuff();
        public static void CreateSword() => CurrentFactory.CreateSword();

        public static void SetFactory(string factoryType)
        {
            CurrentFactory = factoryType.ToLower() switch
            {
                "Red" => new RedFactory(),
                "Blue" => new BlueFactory(),
                _ => CurrentFactory
            };
        }
    }
}
