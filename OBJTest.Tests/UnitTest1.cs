using System;
using System.Collections.Generic;
using System.IO;
using DragonBones;
using OBJTest.Content.NPCs.BONES;
using Xunit;

namespace OBJTest.Tests;

public class DragonBonesAssetLoadingTests
{
    private sealed class TestTextureAtlasData : TextureAtlasData
    {
        public override TextureData CreateTexture() => new TestTextureData();
    }

    private sealed class TestTextureData : TextureData
    {
        protected override void _OnClear() => base._OnClear();
    }

    private static string AssetPath(string fileName) =>
        Path.Combine(AppContext.BaseDirectory, fileName);

    [Fact]
    public void SkeletonBinary_Loads_AndContainsArmature()
    {
        var skePath = AssetPath("NewProject_ske.dbbin");
        Assert.True(File.Exists(skePath), $"Missing test asset: {skePath}");

        var bytes = File.ReadAllBytes(skePath);
        Assert.True(bytes.Length > 0);

        // DragonBones .dbbin is expected to start with ASCII "DBDT" in the first 4 bytes.
        Assert.True(bytes.Length >= 4);
        var tag = new byte[] { bytes[0], bytes[1], bytes[2], bytes[3] };
        Assert.Equal(new byte[] { (byte)'D', (byte)'B', (byte)'D', (byte)'T' }, tag);

        var binaryParser = new BinaryDataParser();
        int headerLength;
        var header = BinaryDataParser.DeserializeBinaryJsonData(bytes, out headerLength);
        Assert.NotNull(header);
        Assert.True(headerLength > 0);
        Assert.True(header.ContainsKey("version"), "Binary header missing 'version'.");
        var version = header["version"]?.ToString() ?? "";
        Assert.False(string.IsNullOrWhiteSpace(version));

        var dbData = binaryParser.ParseDragonBonesData(bytes, scale: 1.0f);
        Assert.True(dbData != null, $"ParseDragonBonesData returned null. Header version='{version}'.");
        Assert.False(string.IsNullOrWhiteSpace(dbData.name));

        Assert.True(dbData.armatureNames.Count > 0, "No armatures found in parsed data.");
        var firstArmatureName = dbData.armatureNames[0];
        var armature = dbData.GetArmature(firstArmatureName);
        Assert.NotNull(armature);
    }

    [Fact]
    public void TextureAtlasJson_ParsesTexturesAndRegions()
    {
        var atlasPath = AssetPath("NewProject_tex.json");
        Assert.True(File.Exists(atlasPath), $"Missing test asset: {atlasPath}");

        var jsonText = File.ReadAllText(atlasPath);
        var jsonData = MiniJSON.Json.Deserialize(jsonText);
        Assert.NotNull(jsonData);

        var jsonParser = new ObjectDataParser();
        var atlasData = new TestTextureAtlasData();
        jsonParser.ParseTextureAtlasData(jsonData, atlasData);

        Assert.False(string.IsNullOrWhiteSpace(atlasData.name));

        var texA = atlasData.GetTexture("BackroomsMinion");
        Assert.NotNull(texA);
        Assert.True(texA.region.width > 0);
        Assert.True(texA.region.height > 0);

        var texB = atlasData.GetTexture("Glock");
        Assert.NotNull(texB);
        Assert.True(texB.region.width > 0);
        Assert.True(texB.region.height > 0);
    }

    [Fact]
    public void AtlasPng_IsValidPngSignature_AndMatchesAtlasJsonImagePath()
    {
        var atlasPath = AssetPath("NewProject_tex.json");
        var pngPath = AssetPath("NewProject_tex.png");
        Assert.True(File.Exists(pngPath), $"Missing test asset: {pngPath}");

        var png = File.ReadAllBytes(pngPath);
        Assert.True(png.Length >= 8);

        // PNG signature: 89 50 4E 47 0D 0A 1A 0A
        Assert.Equal(0x89, png[0]);
        Assert.Equal((byte)'P', png[1]);
        Assert.Equal((byte)'N', png[2]);
        Assert.Equal((byte)'G', png[3]);
        Assert.Equal(0x0D, png[4]);
        Assert.Equal(0x0A, png[5]);
        Assert.Equal(0x1A, png[6]);
        Assert.Equal(0x0A, png[7]);

        var jsonText = File.ReadAllText(atlasPath);
        var obj = MiniJSON.Json.Deserialize(jsonText);
        Assert.NotNull(obj);
        Assert.IsType<Dictionary<string, object>>(obj);

        var dict = (Dictionary<string, object>)obj;
        Assert.True(dict.TryGetValue("imagePath", out var imagePathObj));
        Assert.Equal("NewProject_tex.png", imagePathObj?.ToString());
    }
}
