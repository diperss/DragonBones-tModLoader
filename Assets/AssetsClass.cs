using Microsoft.Xna.Framework.Graphics;
using System.CodeDom.Compiler;
using ReLogic.Content;
using Terraria.Audio;

namespace OBJTest.Assets
{
    public class AssetsClass
    {
        public class Textures
        {
            public class Persona
            {
                public static readonly LazyAsset<Texture2D> PersonaTexture = LazyAsset<Texture2D>.FromPath("OBJTest/Assets/Textures/Persona");
            }
        }
    }
}
