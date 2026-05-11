using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.AbstarctFactoryTest
{
    public abstract class NPCFactory
    {
        public Player player => Main.player[Main.myPlayer];
        public Vector2 worldMouse => Main.MouseWorld;

        public abstract void CreateBow();
        public abstract void CreateStuff();
        public abstract void CreateSword();

        public void CreateNPC(int type, int ai) => NPC.NewNPC(player.GetSource_FromThis(), (int)worldMouse.X, (int)worldMouse.Y, type, ai0: ai);
    }

    public class RedFactory : NPCFactory
    {
        public override void CreateBow() => CreateNPC(ModContent.NPCType<BowNPC>(), 1);
        public override void CreateStuff() => CreateNPC(ModContent.NPCType<StuffNPC>(), 1);
        public override void CreateSword() => CreateNPC(ModContent.NPCType<SwordNPC>(), 1);
    }

    public class BlueFactory : NPCFactory
    {
        public override void CreateBow() => CreateNPC(ModContent.NPCType<BowNPC>(), 2);
        public override void CreateStuff() => CreateNPC(ModContent.NPCType<StuffNPC>(), 2);
        public override void CreateSword() => CreateNPC(ModContent.NPCType<SwordNPC>(), 2);
    }
}
