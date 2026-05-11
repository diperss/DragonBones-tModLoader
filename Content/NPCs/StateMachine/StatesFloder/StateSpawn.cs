using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace OBJTest.Content.NPCs.StateMachine.StatesFolder
{
    
    public class StateSpawn(float timer, int NPCID, float duration = 100) : IStateMachine
    {
        public Vector2 offset1 = new Vector2(0, 50);
        float IStateMachine.timer
        {
            get => timer;
            set => timer = value;
        }

        float IStateMachine.duration => duration;

        NPC _persona => Main.npc[NPCID];

        public void Reset()
        {
            timer = 0;
        }

        public void Update()
        {
            _persona.position = Vector2.SmoothStep(_persona.position, _persona.position + offset1, timer / duration);
        }
    }
}
