using OBJTest.Assets;
using OBJTest.Content.NPCs.StateMachine.StatesFolder;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.StateMachine
{
    
    public class PersonaNPC : ModNPC
    {
        public override string Texture => "OBJTest/Assets/Textures/Persona";
        private PersonaStateMachine _stateMachine;

        public float CurrentPhaseMode
        {
            get => (int)NPC.ai[0];
            set => NPC.ai[0] = value;
        }

        public float AIState
        {
            get => (int)NPC.ai[1];
            set => NPC.ai[1] = value;
        }

        public float CurrentAttack
        {
            get => (int)NPC.ai[2];
            set => NPC.ai[2] = value;
        }

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.friendly = false;
            NPC.aiStyle = -1;
            NPC.damage = 5;
            NPC.defense = 5;
            NPC.lifeMax = 100;
            NPC.width = 50;
            NPC.height = 50;
        }

        public override void OnSpawn(IEntitySource source)
        {
            _stateMachine = new PersonaStateMachine(NPC.whoAmI);
            _stateMachine.InitializeStates();
        }

        public override void AI()
        {
            _stateMachine?.Update(AIState);
            AIState += 1f; 
        }
    }
}