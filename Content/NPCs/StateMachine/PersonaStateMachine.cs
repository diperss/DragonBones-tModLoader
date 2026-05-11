using ReLogic.Content;
using System;
using System.Collections.Generic;

namespace OBJTest.Content.NPCs.StateMachine
{
    /// <summary>
    /// Enum with all the states for PersonaNPC
    /// </summary>
    public enum PersonaState
    {
        Spawn,
        Idle,
        Minigun,
        Snipe,
        SwordCombo
    }

    /// <summary>
    /// State machine controller for Persona NPC that manages behavioral states (Spawn, Idle, SwordCombo, etc.).
    /// This class used for:
    /// Initialization of states,
    /// Updating state timers,
    /// Calling a reset on the current state,
    /// Forced and normal state transitions
    /// </summary>
    public class PersonaStateMachine
    {
        private readonly int _npcID;
        private IStateMachine _currentState;
        private readonly Dictionary<PersonaState, IStateMachine> _states;

        public IStateMachine CurrentState => _currentState;

        public PersonaStateMachine(int npcID)
        {
            _npcID = npcID;
            _states = new Dictionary<PersonaState, IStateMachine>();
        }

        
        // Lazy initialization
        private string folder_of_states = "OBJTest.Content.NPCs.StateMachine.StatesFolder";
        public void InitializeStates()
        {
            var stateTypes = new Dictionary<PersonaState, Type>();
            foreach (PersonaState state in Enum.GetValues(typeof(PersonaState)))
            {
                string typeName = folder_of_states + $".State{state}";
                Type type = Type.GetType(typeName);

                if (type != null)
                {
                    stateTypes[state] = type;
                }
            }
            foreach (var kvp in stateTypes)
            {
                _states[kvp.Key] = (IStateMachine)Activator.CreateInstance(kvp.Value, 0, _npcID,100);
            }
        }

        public void Update(float currentTimer)
        {
            if (_currentState == null) return;

            _currentState.timer = currentTimer;

            if (_currentState.timer >= _currentState.duration)
            {
                SwitchToNextState();
            }
        }

        private void SwitchToNextState()
        {
            _currentState.Reset();
            PersonaState nextState = DetermineNextState();
            _currentState = _states[nextState];
        }

        private PersonaState DetermineNextState()
        {
            return PersonaState.Spawn;
        }

        public void ForceSwitchState(PersonaState newState)
        {
            if (_states.ContainsKey(newState))
            {
                _currentState?.Reset();
                _currentState = _states[newState];
            }
        }
    }
}