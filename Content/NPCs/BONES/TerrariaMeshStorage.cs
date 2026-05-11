using System;
using DragonBones;
using Microsoft.Xna.Framework.Graphics;

namespace OBJTest.Content.NPCs.BONES
{
    /// <summary>
    /// Статический кэш индексов и UV; динамический VB для позиций/цвета (скиннинг и деформация на CPU).
    /// </summary>
    public sealed class TerrariaMeshStorage : IDisposable
    {
        public DynamicVertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }
        public int VertexCount { get; private set; }
        public int PrimitiveCount { get; private set; }
        /// <summary>Позиции в пространстве арматуры (обновляются в <c>_UpdateMesh</c>).</summary>
        public VertexPositionColorTexture[] Scratch { get; private set; }
        /// <summary>Копия для <c>SetData</c> с цветом/экранными координатами на кадр отрисовки.</summary>
        public VertexPositionColorTexture[] UploadScratch { get; private set; }

        public static TerrariaMeshStorage TryCreate(GraphicsDevice device, VerticesData vd)
        {
            if (device == null || vd?.data == null)
                return null;

            var intArray = vd.data.intArray;
            var floatArray = vd.data.floatArray;
            if (intArray == null || floatArray == null)
                return null;

            int meshOffset = vd.offset;
            int vCount = intArray[meshOffset + (int)BinaryOffset.MeshVertexCount];
            int triCount = intArray[meshOffset + (int)BinaryOffset.MeshTriangleCount];
            if (vCount <= 0 || triCount <= 0)
                return null;

            int floatVertexOffset = intArray[meshOffset + (int)BinaryOffset.MeshFloatOffset];
            if (floatVertexOffset < 0)
                floatVertexOffset += 65536;

            int uvOffset = floatVertexOffset + vCount * 2;
            int indexCount = triCount * 3;
            var indices = new ushort[indexCount];
            for (int i = 0; i < indexCount; i++)
            {
                int idx = intArray[meshOffset + (int)BinaryOffset.MeshVertexIndices + i];
                if (idx < 0)
                    idx += 65536;
                if (idx > ushort.MaxValue)
                    return null;
                indices[i] = (ushort)idx;
            }

            var scratch = new VertexPositionColorTexture[vCount];
            for (int i = 0; i < vCount; i++)
            {
                int i2 = i * 2;
                float u = floatArray[uvOffset + i2];
                float v = floatArray[uvOffset + i2 + 1];
                scratch[i] = new VertexPositionColorTexture(
                    new Microsoft.Xna.Framework.Vector3(0, 0, 0),
                    Microsoft.Xna.Framework.Color.White,
                    new Microsoft.Xna.Framework.Vector2(u, v));
            }

            var vb = new DynamicVertexBuffer(device, typeof(VertexPositionColorTexture), vCount, BufferUsage.WriteOnly);
            var ib = new IndexBuffer(device, IndexElementSize.SixteenBits, indexCount, BufferUsage.WriteOnly);
            ib.SetData(indices);
            vb.SetData(scratch);

            return new TerrariaMeshStorage
            {
                VertexBuffer = vb,
                IndexBuffer = ib,
                VertexCount = vCount,
                PrimitiveCount = triCount,
                Scratch = scratch,
                UploadScratch = new VertexPositionColorTexture[vCount]
            };
        }

        public void UploadFromUploadScratch()
        {
            VertexBuffer.SetData(UploadScratch);
        }

        public void Dispose()
        {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();
            VertexBuffer = null;
            IndexBuffer = null;
            Scratch = null;
            UploadScratch = null;
            VertexCount = 0;
            PrimitiveCount = 0;
        }
    }
}
