using DragonBones;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OBJTest.Content.NPCs.BONES;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

public class TestDragonNPC : ModNPC
{    
    // dummy texture
    public override string Texture => "Terraria/Images/NPC_0";

    /* all animations called from here */
    private Armature _armature;
    private string _currentAnim = "";
    private bool _armatureBuildFailed;

    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 64;
        NPC.damage = 10;
        NPC.defense = 5;
        NPC.lifeMax = 100;
        NPC.value = 100f;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.5f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        EnsureArmature();
    }

    /* If called early than a "AnimationManager : ModSystem" - return nothing.
    Gets a first armature contained in AnimationManager.
    Builds the armature.
    (used in unit tests) If all of this completes successfully, but the armature is null: _armatureBuildFailed = true; 
    _armature is assigned one time in NPC code */
    private void EnsureArmature()
    {
        if (_armature != null || _armatureBuildFailed)
            return;

        if (TerrariaDBFactory.Instance == null || AnimationManager.DBdata == null || AnimationManager.AtlasData == null)
            return;

        var armatureName = AnimationManager.PrimaryArmatureName;
        if (string.IsNullOrEmpty(armatureName))
            return;

        _armature = TerrariaDBFactory.Instance.BuildArmature(
            armatureName: armatureName,
            dragonBonesName: AnimationManager.DBdata.name,
            skinName: null,
            textureAtlasName: AnimationManager.AtlasData.name
        );

        if (_armature == null)
            _armatureBuildFailed = true;
    }

    // Gets a first animation (better set an Enum for this and call from _animation for wilder use).
    // _armature.AdvanceTime(speed) is required to make the armature move.

    public override void AI()
    {
        float speed = (1f / 60f) * 0.2f;
        if (_armature != null)
        {
            var animName = AnimationManager.PrimaryAnimationName;
            if (!string.IsNullOrEmpty(animName) && _currentAnim != animName)
            {
                _armature.animation.Play(animName);
                _currentAnim = animName;
            }
            
            _armature.AdvanceTime(speed);
        }
    }

    // Return false to prevent vanilla drawing
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        EnsureArmature();
        if (_armature != null && _armature.parent == null)
        {
            TerrariaSlot.DrawArmature(
                _armature,
                spriteBatch,
                NPC.Center - screenPos,
                NPC.spriteDirection == 1,
                NPC.scale,
                drawColor);
        }
        return false;
    }

    public override void OnKill()
    {
        _armature?.Dispose();
        _armature = null;
        _armatureBuildFailed = false;
    }
}