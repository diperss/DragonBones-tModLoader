using DragonBones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace OBJTest.Content.NPCs.BONES
{
    // Класс для отображения текстуры. Display в этом коде означает "отображать", а не "монитор".
    public class TerrariaSlot : Slot
    {
        private TerrariaTextureData _currentTextureData;
        private Color _currentColor = Color.White;
        private bool _isVisible = true;
        private BlendMode _currentBlendMode = BlendMode.Normal;

        private TerrariaMeshStorage _meshGpuCache;
        private VerticesData _meshCacheVerticesData;

        
        #region Override metods (Many of them used for creating a custom editor)
        #region Display aka what and how to draw (Texture/Mesh ect)

        protected override void _InitDisplay(object value, bool isRetain)
        {
            if (value is TerrariaTextureData texData)
            {
                _currentTextureData = texData;
            }
        }

        /// <summary>
        /// Dispose here if having errors on Unload
        /// </summary>
        protected override void _DisposeDisplay(object value, bool isRelease)
        {
            if (value is TerrariaTextureData texData && isRelease)
            {
            }
            _currentTextureData = null;
        }


        protected override void _OnUpdateDisplay()
        {
            // Здесь можно обновить внутренние флаги, если нужно
        }

        protected override void _AddDisplay() { }

        /// <summary>
        /// Changing texture data.
        /// </summary>
        protected override void _ReplaceDisplay(object value)
        {
            if (value is TerrariaTextureData texData)
                _currentTextureData = texData;
            else
                _currentTextureData = null;
        }

        protected override void _RemoveDisplay()
        {
            _currentTextureData = null;
        }

        #endregion

        protected override void _UpdateZOrder()
        {
            
        }

        internal override void _UpdateVisible()
        {
            _isVisible = true;
        }

        // For VFX
        // TODO: transform blend mode to XNA BlendState
        internal override void _UpdateBlendMode()
        {
            _currentBlendMode = _blendMode;
        }

        protected override void _UpdateColor() {}

        // For "Show texture" in DB properties. Can be animated.
        protected override void _UpdateFrame()
        {
            _currentTextureData = _textureData as TerrariaTextureData;
        }

        /// <summary>
        /// Скиннинг и деформация по рантайму DragonBones (см. UnitySlot._UpdateMesh в DragonBonesCSharp).
        /// В <see cref="Scratch"/> пишутся позиции в пространстве арматуры; в отрисовке умножаются на масштаб/якорь.
        /// </summary>
        protected override void _UpdateMesh()
        {
            if (_armature == null || _deformVertices == null || _deformVertices.verticesData == null)
                return;

            var vd = _deformVertices.verticesData;
            var gd = Main.graphics.GraphicsDevice;
            if (gd == null)
                return;

            if (_meshCacheVerticesData != vd)
            {
                _meshGpuCache?.Dispose();
                _meshGpuCache = TerrariaMeshStorage.TryCreate(gd, vd);
                _meshCacheVerticesData = vd;
            }

            if (_meshGpuCache == null)
                return;

            var intArray = vd.data.intArray;
            var floatArray = vd.data.floatArray;
            var deformList = _deformVertices.vertices;
            var bones = _deformVertices.bones;
            var scratch = _meshGpuCache.Scratch;
            int vertexCount = _meshGpuCache.VertexCount;
            float armatureScale = _armature.armatureData.scale;

            var weightData = vd.weight;
            if (weightData != null)
            {
                int weightFloatOffset = intArray[weightData.offset + (int)BinaryOffset.WeigthFloatOffset];
                if (weightFloatOffset < 0)
                    weightFloatOffset += 65536;

                bool hasDeform = deformList.Count > 0;
                int iB = weightData.offset + (int)BinaryOffset.WeigthBoneIndices + weightData.bones.Count;
                int iV = weightFloatOffset;
                int iF = 0;

                for (int i = 0; i < vertexCount; ++i)
                {
                    int boneCount = intArray[iB++];
                    float xG = 0.0f, yG = 0.0f;
                    for (int j = 0; j < boneCount; ++j)
                    {
                        int boneListIndex = intArray[iB++];
                        float w = floatArray[iV++];
                        float xL = floatArray[iV++] * armatureScale;
                        float yL = floatArray[iV++] * armatureScale;
                        if (hasDeform)
                        {
                            xL += deformList[iF++];
                            yL += deformList[iF++];
                        }

                        if (boneListIndex >= 0 && boneListIndex < bones.Count && bones[boneListIndex] != null)
                        {
                            DragonBones.Matrix matrix = bones[boneListIndex].globalTransformMatrix;
                            xG += (matrix.a * xL + matrix.c * yL + matrix.tx) * w;
                            yG += (matrix.b * xL + matrix.d * yL + matrix.ty) * w;
                        }
                    }

                    scratch[i].Position = new Vector3(xG, yG, 0f);
                }
            }
            else
            {
                int vertexOffset = intArray[vd.offset + (int)BinaryOffset.MeshFloatOffset];
                if (vertexOffset < 0)
                    vertexOffset += 65536;

                DragonBones.Matrix m = globalTransformMatrix;
                float a = m.a, b = m.b, c = m.c, d = m.d, tx = m.tx, ty = m.ty;

                if (deformList.Count > 0)
                {
                    for (int i = 0, iV = 0, iF = 0; i < vertexCount; ++i)
                    {
                        float rx = (floatArray[vertexOffset + iV++] * armatureScale + deformList[iF++]);
                        float ry = (floatArray[vertexOffset + iV++] * armatureScale + deformList[iF++]);
                        float vx = rx * a + ry * c + tx;
                        float vy = rx * b + ry * d + ty;
                        scratch[i].Position = new Vector3(vx, vy, 0f);
                    }
                }
                else
                {
                    for (int i = 0, iV = 0; i < vertexCount; ++i)
                    {
                        float rx = floatArray[vertexOffset + iV++] * armatureScale;
                        float ry = floatArray[vertexOffset + iV++] * armatureScale;
                        float vx = rx * a + ry * c + tx;
                        float vy = rx * b + ry * d + ty;
                        scratch[i].Position = new Vector3(vx, vy, 0f);
                    }
                }
            }
        }

        protected override void _UpdateTransform() { }

        // IDK what this is for
        protected override void _IdentityTransform() { }
        #endregion

        // TODO: Draw texture of armature.
        /// <summary>
        /// Used for draw only "no parents :(" slots. Call draw for all children recursively.
        /// </summary>
        /// <param name="anchorScreen"> in frame rectangle coordinates </param>
        public static void DrawArmature(Armature armature, SpriteBatch spriteBatch, Vector2 anchorScreen, bool flipWorldX, float objectScale, Color tint)
        {
            if (armature == null)
                return;

            foreach (Slot slot in armature.GetSlots())
            {
                if (slot is TerrariaSlot terrariaSlot)
                    terrariaSlot.DrawSlotRecursive(spriteBatch, anchorScreen, flipWorldX, objectScale, tint);
            }
        }

        // TODO: Tint
        /// <summary> Draws whatever is in the slot. </summary>
        /// <param name="anchorScreen"> in frame rectangle coordinates </param>
        private void DrawSlotRecursive(SpriteBatch spriteBatch, Vector2 anchorScreen, bool flipWorldX, float objectScale, Color tint)
        {
            bool drewMesh = false;
            if (_display == _meshDisplay && _meshGpuCache != null && _currentTextureData?.Texture != null
                && (DbMeshRenderSystem.BasicTextured != null || DbMeshRenderSystem.FxTextured != null))
            {
                // draw mesh
                DrawSkinnedMesh(spriteBatch, anchorScreen, flipWorldX, objectScale, tint);
                drewMesh = true;
            }

            // draw texture
            if (!drewMesh && _currentTextureData?.Texture != null)
            {
                // GET PARAMETERS
                DragonBones.Rectangle region = _currentTextureData.region;
                Microsoft.Xna.Framework.Rectangle sourceRect = new Microsoft.Xna.Framework.Rectangle(
                    (int)region.x,
                    (int)region.y,
                    (int)region.width,
                    (int)region.height
                );
                DragonBones.Matrix m = globalTransformMatrix; // | a  c tx |    a,b - new vector for X
                                                              // | b  d ty |    c,d - new vector for Y
                                                              // | 0  0  1 |    {0,0} -> {tx, ty} - shift of anchor 

                float scaleX = (float)Math.Sqrt(m.a * m.a + m.b * m.b);
                float scaleY = (float)Math.Sqrt(m.c * m.c + m.d * m.d);
                float rotation = (float)Math.Atan2(m.b, m.a);

                // X-FLIP
                SpriteEffects effects = flipWorldX ? SpriteEffects.FlipVertically : SpriteEffects.None;
                Vector2 origin = new Vector2(flipWorldX ? region.width - _pivotX : _pivotX, _pivotY);
                Vector2 finalScale = new Vector2(scaleX, scaleY) * objectScale;
                Vector2 position = anchorScreen + new Vector2(flipWorldX ? -m.tx : m.tx, m.ty) * objectScale;
                rotation = flipWorldX ? float.Pi - rotation : rotation;

                // DRAWING
                spriteBatch.Draw(
                    _currentTextureData.Texture,
                    position,
                    sourceRect,
                    _currentColor,
                    rotation,
                    origin,
                    finalScale,
                    effects,
                    0f
                );
            }

            if (childArmature != null)
                DrawArmature(childArmature, spriteBatch, anchorScreen, flipWorldX, objectScale, tint);
        }

        private void DrawSkinnedMesh(SpriteBatch spriteBatch, Vector2 anchorScreen, bool flipWorldX, float objectScale, Color tint)
        {
            var cache = _meshGpuCache;
            var scratch = cache.Scratch;
            var upload = cache.UploadScratch;
            var device = spriteBatch.GraphicsDevice;

            var vc = Color.White;

            // UPDATE "upload" PARAMETERS
            DragonBones.Rectangle region = _currentTextureData.region;
            Texture2D atlas = _currentTextureData.Texture;

            float stepU = 1f / atlas.Width;
            float stepV = 1f / atlas.Height;

            for (int i = 0; i < cache.VertexCount; i++)
            {
                float x = scratch[i].Position.X;
                float y = scratch[i].Position.Y;

                upload[i].Position = new Vector3(
                    anchorScreen.X + (flipWorldX ? -x : x) * objectScale,
                    anchorScreen.Y + y * objectScale,
                    0f);

                float u = scratch[i].TextureCoordinate.X;
                float v = scratch[i].TextureCoordinate.Y;

                upload[i].TextureCoordinate = new Vector2(
                    (region.x + u * region.width) * stepU,
                    (region.y + v * region.height) * stepV
                );

                upload[i].Color = vc;
            }
        

            cache.UploadFromUploadScratch();

            var blend = device.BlendState;
            var depth = device.DepthStencilState;
            var raster = device.RasterizerState;
            var sampler0 = device.SamplerStates[0];

            spriteBatch.End();
            try
            {
                device.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend;
                device.DepthStencilState = DepthStencilState.None;
                device.RasterizerState = RasterizerState.CullNone;
                device.SamplerStates[0] = SamplerState.LinearClamp;

                device.SetVertexBuffer(cache.VertexBuffer);
                device.Indices = cache.IndexBuffer;

                Microsoft.Xna.Framework.Matrix wvp = Main.GameViewMatrix.TransformationMatrix * Microsoft.Xna.Framework.Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                DbMeshRenderSystem.ApplyTexturedPass(_currentTextureData.Texture, wvp);

                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    cache.VertexCount,
                    0,
                    cache.PrimitiveCount);
            }
            finally
            {
                device.BlendState = blend;
                device.DepthStencilState = depth;
                device.RasterizerState = raster;
                device.SamplerStates[0] = sampler0;

                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullCounterClockwise,
                    null,
                    Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}