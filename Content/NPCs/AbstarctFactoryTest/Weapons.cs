using Microsoft.Build.Tasks.Deployment.ManifestUtilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.AbstarctFactoryTest
{
    public abstract class Weapons : ModNPC
    {

        public override string Texture => "OBJTest/Content/NPCs/AbstarctFactoryTest/32x32_PixelWeapons_Free";

        protected abstract int WeaponColumn { get; }
        
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
            
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.damage = 50;
            NPC.lifeMax = 100;
            NPC.defense = 10;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY);
            Rectangle sourceRectangle = NPC.frame;

            Main.EntitySpriteDraw(texture, drawPosition, sourceRectangle, Color.White, 0f, sourceRectangle.Size() / 2f, NPC.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frame.Width = 32;
            NPC.frame.Height = 32;
            NPC.frame.Y = NPC.frame.Height * (int)NPC.ai[0];
            NPC.frame.X = NPC.frame.Width * WeaponColumn;
        }
    }
    public class BowNPC : Weapons
    {
        protected override int WeaponColumn => 3;
    }
    public class SwordNPC : Weapons
    {
        protected override int WeaponColumn => 0;
    }
    public class StuffNPC : Weapons
    {
        protected override int WeaponColumn => 2;
    }

}


