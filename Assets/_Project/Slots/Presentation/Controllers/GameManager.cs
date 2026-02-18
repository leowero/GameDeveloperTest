using Project.Core.SlotMachine;
using Project.Core.SlotMachine.States;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Slots.Presentation.Controllers
{
    public class GameManager : MonoBehaviour
    {
        private GameStateMachine StateMachine = new GameStateMachine();
        private List<IGameState> GameStates = new List<IGameState>();

        private void Start()
        {
            GameStates.Add(new StartState());
            GameStates.Add(new SpinState());
            GameStates.Add(new EndState());

            foreach (IGameState gameState in GameStates)
            {
                StateMachine.RegisterState(gameState);
            }
        }

        private void Update()
        {
            StateMachine.Update();
        }
    }
}