using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.BONES
{
    /// <summary>
    /// Общий BasicEffect для отрисовки DragonBones-меша; опционально <see cref="FxTextured"/> из скомпилированного DBSkinnedMesh.xnb.
    /// </summary>
    public sealed class DbMeshRenderSystem : ModSystem
    {
        public static BasicEffect BasicTextured { get; private set; }
        public static Effect FxTextured { get; private set; }

        public override void Load()
        {
            if (Main.dedServ)
                return;

            var gd = Main.graphics.GraphicsDevice;
            Main.RunOnMainThread(()=> 
            BasicTextured = new BasicEffect(gd)
            {
                TextureEnabled = true,
                VertexColorEnabled = true,
                World = Matrix.Identity,
                View = Matrix.Identity
            });

            if (Mod != null)
            {
                try
                {
                    FxTextured = ModContent.Request<Effect>("Content/Effects/DBSkinnedMesh").Value;
                }
                catch
                {
                    FxTextured = null;
                }
            }
        }

        public override void Unload()
        {
            BasicTextured?.Dispose();
            BasicTextured = null;
            FxTextured = null;
        }
        public static void ApplyTexturedPass(Texture2D texture, Matrix worldViewProjection)
        {
            if (FxTextured != null)
            {
                FxTextured.Parameters["WorldViewProjection"]?.SetValue(worldViewProjection);
                FxTextured.Parameters["MatrixTransform"]?.SetValue(worldViewProjection);
                FxTextured.Parameters["Texture"]?.SetValue(texture);
                foreach (EffectPass p in FxTextured.CurrentTechnique.Passes)
                    p.Apply();
                return;
            }
            else if (BasicTextured != null)
            {
                BasicTextured.Texture = texture;
                BasicTextured.World = Matrix.Identity;
                BasicTextured.View = Matrix.Identity;
                BasicTextured.Projection = worldViewProjection;
                foreach (EffectPass p in BasicTextured.CurrentTechnique.Passes)
                    p.Apply();
            }
        }
    }
}
