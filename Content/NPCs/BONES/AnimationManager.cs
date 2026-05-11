using DragonBones;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.BONES
{
    // ModSystem calss for initaizing one DragonBones data for one type of NPC
    public class AnimationManager : ModSystem
    {
        private string DBskeletonPath = "OBJTest/Assets/NPCAnimationTest/1/NewProject_ske.dbbin";
        private string DBAtlasPath = "OBJTest/Assets/NPCAnimationTest/1/NewProject_tex.json";
        private string DBAtlasTexturePath = "OBJTest/Assets/NPCAnimationTest/1/NewProject_tex";

        public static DragonBonesData DBdata { get; private set; }
        public static TerrariaTextureAtlasData AtlasData { get; private set; } // Contains frames on texture
        public static string PrimaryArmatureName { get; private set; }
        public static string PrimaryAnimationName { get; private set; }

        public Texture2D AtlasTexture { get; private set; }

        public override void Load()
        {
            // INITIALIZING
            TerrariaDBFactory.Initialize();
            AtlasTexture = ModContent.Request<Texture2D>(DBAtlasTexturePath, AssetRequestMode.ImmediateLoad).Value;

            // PARSING
            DB_Parser parser = new DB_Parser(ReadModBytes, DBskeletonPath); // Automatically starts parsing DragonBones data into parser.dragonBonesData
            parser.ParseAtlasData(DBAtlasPath);

            // PACKING
            DBdata = parser.dragonBonesData;
            AtlasData = parser.textureAtlasData;
            PrimaryArmatureName = ResolvePrimaryArmatureName(DBdata);
            PrimaryAnimationName = ResolvePrimaryAnimationName(DBdata, PrimaryArmatureName);
            if (DBdata != null)
            {
                TerrariaDBFactory.Instance.AddDragonBonesData(DBdata, DBdata.name);
            }
            if (AtlasData != null)
            {
                AtlasData.AtlasTexture = AtlasTexture;
                TerrariaDBFactory.Instance.AddTextureAtlasData(AtlasData, AtlasData.name);
            }
        }
        public override void Unload()
        {
            TerrariaDBFactory.Instance?.Clear(true);
            DBdata = null;
            AtlasData = null;
            PrimaryArmatureName = null;
            PrimaryAnimationName = null;
        }

        private byte[] ReadModBytes(string assetPath)
        {
            // Files inside .tmod are stored without the "ModName/" prefix.
            var insideModPath = assetPath.StartsWith(Mod.Name + "/", StringComparison.OrdinalIgnoreCase)
                ? assetPath.Substring(Mod.Name.Length + 1)
                : assetPath;

            return Mod.GetFileBytes(insideModPath);
        }

        private static string ResolvePrimaryArmatureName(DragonBonesData data)
        {
            if (data == null || data.armatureNames.Count == 0)
                return null;
            return data.armatureNames[0];
        }

        private static string ResolvePrimaryAnimationName(DragonBonesData data, string armatureName)
        {
            if (data == null || string.IsNullOrEmpty(armatureName))
                return null;

            var armature = data.GetArmature(armatureName);
            if (armature == null || armature.animations == null || armature.animations.Count == 0)
                return null;

            return armature.animations.Keys.FirstOrDefault();
        }
    }


}
