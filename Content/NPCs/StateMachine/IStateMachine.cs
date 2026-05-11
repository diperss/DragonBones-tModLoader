using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBJTest.Content.NPCs.StateMachine
{
    public interface IStateMachine
    {
        float timer { get; set; }
        float duration { get; }
        public abstract void Reset();
        public abstract void Update();

    }
}
