using DragonBones;
using Microsoft.Xna.Framework.Graphics;

namespace OBJTest.Content.NPCs.BONES
{
    public class TerrariaTextureData : TextureData
    {
        public Texture2D Texture { get; set; }

        protected override void _OnClear()
        {
            base._OnClear();
            Texture = null;
        }
    }
}
