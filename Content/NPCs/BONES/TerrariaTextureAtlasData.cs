using DragonBones;
using Microsoft.Xna.Framework.Graphics;

namespace OBJTest.Content.NPCs.BONES
{
    public class TerrariaTextureAtlasData : TextureAtlasData
    {
        private Texture2D _atlasTexture;

        public Texture2D AtlasTexture
        {
            get => _atlasTexture;
            set
            {
                _atlasTexture = value;
                foreach (var tex in textures.Values)
                {
                    if (tex is TerrariaTextureData terrariaTex)
                    {
                        terrariaTex.Texture = _atlasTexture;
                    }
                }
            }
        }

        public override TextureData CreateTexture()
        {
            return BorrowObject<TerrariaTextureData>();
        }
    }
}