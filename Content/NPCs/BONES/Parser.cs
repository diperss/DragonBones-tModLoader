using DragonBones;
using System;
using System.IO;
using System.Text;

namespace OBJTest.Content.NPCs.BONES
{
    // Used for reading DragonBones and TextureAtlas data
    public class DB_Parser
    {
        public DragonBonesData dragonBonesData { get; private set; }
        public TerrariaTextureAtlasData textureAtlasData { get; private set; }

        private readonly Func<string, byte[]> _readBytesExternal;

        public DB_Parser(Func<string, byte[]> readBytesExternal, string path)
        {
            _readBytesExternal = readBytesExternal;

            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".json":
                    ParseJSONSkeletonData(path);
                    break;
                case ".dbbin":
                    ParseBinarySkeletonData(path);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported format: {extension}");
            }
        }
        private void ParseBinarySkeletonData(string path)
        {
            try
            {
                BinaryDataParser binaryParser = new BinaryDataParser();

                byte[] bytes = ReadBytes(path);
                dragonBonesData = binaryParser.ParseDragonBonesData(bytes, scale: 1.0f);
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось загрузить DragonBones данные");
                Console.WriteLine(e.Message);
            }
        }

        public void ParseJSONSkeletonData(string jsonPath)
        {
            try
            {
                ObjectDataParser jsonParser = new ObjectDataParser();
                var jsonData = JsonPathToObject(jsonPath);
                dragonBonesData = jsonParser.ParseDragonBonesData(jsonData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось загрузить DragonBones данные");
                Console.WriteLine(e.Message);
            }
        }

        public void ParseAtlasData(string atlasPath)
        {
            try
            {
                ObjectDataParser jsonParser = new ObjectDataParser();
                var jsonData = JsonPathToObject(atlasPath);
                textureAtlasData = new TerrariaTextureAtlasData();
                jsonParser.ParseTextureAtlasData(jsonData, textureAtlasData);
            }
            catch (Exception e)
            {
                Console.WriteLine("Не удалось загрузить TextureAtlas данные");
                Console.WriteLine(e.Message);
            }
        }

        public void Unload()
        {
            dragonBonesData.ReturnToPool();
            textureAtlasData.ReturnToPool();
        }

        private object JsonPathToObject(string path)
        {
            string jsonText = ReadText(path);
            var jsonData = MiniJSON.Json.Deserialize(jsonText);
            return jsonData;
        }

        private byte[] ReadBytes(string path)
        {
            // Dev fallback (from file system)
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
            }

            // External provider (e.g. unit tests or tModLoader Mod.GetFileBytes)
            if (_readBytesExternal != null)
            {
                return _readBytesExternal(path);
            }

            throw new FileNotFoundException($"File not found on disk and no external provider set: {path}");
        }

        private string ReadText(string path)
        {
            var bytes = ReadBytes(path);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}