using Project.Core.SlotMachine;
using Project.Core.SlotMachine.States;
using Project.Slots.Domain.Configuration.Definitions;
using Project.Slots.Domain.Symbols;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Slots.Presentation.Controllers
{
    public class GameManager : MonoBehaviour
    {
        public List<Image> Symbols = new List<Image>();
        public List<VisualSymbol> VisualSymbolsData = new List<VisualSymbol>();
        public SymbolType[][] Grid;

        private GameStateMachine StateMachine = new GameStateMachine();

        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            StateMachine.RegisterState(new StartState());
            StateMachine.RegisterState(new SpinState());
            StateMachine.RegisterState(new EndState());

            StateMachine.ChangeState<StartState>();
        }

        private void Update()
        {
            
        }

        public void Spin()
        {
            StateMachine.ChangeState<SpinState>();
            StateMachine.Action();
            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            for (int column = 0; column < Grid.Length; column++)
            {
                for (int row = 0; row < Grid[column].Length; row++)
                {
                    Symbols[column * SlotDefinition.Rows + row].sprite = VisualSymbolsData.FirstOrDefault(s => s.id == Grid[column][row].Type).sprite;
                }
            }
        }
    }
}