using System;
using System.Collections.Generic;
using Core.Interfaces;
using Events;
using Events.EventTypes;
using Gameplay.Wheel.Helper;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using SubSystems.SaveSystem;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Gameplay.Wheel
{
    public abstract class Wheel : MonoBehaviour,IWheel,ISaveable<WheelSaveData>
    {
        public string SaveKey => "WheelDataSave";
        
        [Header("Referances")] 
        [SerializeField] private BallController ballController;
        [SerializeField] protected Transform ball;
        [SerializeField] protected Transform wheelTransform;
        [SerializeField] protected List<Transform> diamonds;
        
        [Header("Settings")]
        [SerializeField] protected float wheelRotateSpeed = 0.2f;
        [Range(0f, 15f)]
        [SerializeField]protected float dividerAngle = 1.0f;
        [SerializeField] protected float slotPosWorldRadius = 0.9f;
        
        [SerializeField] protected Vector3 WheelCenter = Vector3.zero;
        private readonly List<Transform> _jumpPointTransforms = new();
        private readonly Vector3 _axis = Vector3.down;
        private Transform[] _slotTransforms; // Array holding the transforms of each slot on the wheel, mapped by European order
        protected int[] SlotOrder; // The sequence of pocket numbers in European roulette order (important for slot placement)
        
        [Header("Wheel Settings")]
        public float spinSpeed = 180f; // Degrees per second that wheel spins
        
        private bool _isSpinning;
        private int _predeterminedNumber;

        private void OnEnable()
        {
            // Subscribe to game state change event when enabled
            EventBus<GameStateChangedEvent>.Subscribe(OnGameStateChanged);
            EventBus<DeterminedNumberSelectedEvent>.Subscribe(OnDeterminedNumberSelected,true);
        }

        private void OnDisable()
        {
            // Unsubscribe to avoid memory leaks when disabled
            EventBus<GameStateChangedEvent>.Unsubscribe(OnGameStateChanged);
            EventBus<DeterminedNumberSelectedEvent>.Unsubscribe(OnDeterminedNumberSelected);
        }
        
        private void OnDeterminedNumberSelected(DeterminedNumberSelectedEvent args)
        {
            _predeterminedNumber = args.SelectedNumber;
        }
        
        private void OnGameStateChanged(GameStateChangedEvent args)
        {
            // React to game state changes, start spinning when game enters Spinning state
            switch (args.CurrentState)
            {
                case GameState.Spinning:
                    if(_isSpinning) return;
                    Spin();
                    break;
                case GameState.Betting:
                    ballController.IdleRoutine();
                    SetRandomDeterminationNumber();
                    break;
                case GameState.Result:
                    break;
            }
        }

        private void SetRandomDeterminationNumber()
        {
            EventBus<DeterminedNumberSelectedEvent>.Raise(new DeterminedNumberSelectedEvent
            {
                SelectedNumber = Random.Range(0,RouletteType.ToNumberCount())
            });
        }

        protected virtual void Awake()
        {
            // Initialize the slot transforms array based on the number of pockets
            _slotTransforms = new Transform[SlotOrder.Length];

            // Create a parent object to organize slot transforms under the wheel
            var slotsParent = new GameObject("Slots").transform;
            slotsParent.SetParent(wheelTransform);
            slotsParent.localPosition = Vector3.zero;

            // For each pocket, create an empty GameObject at its calculated world position
            for (int i = 0; i < SlotOrder.Length; i++)
            {
                var newSlotTr = new GameObject($"SlotNumber_{SlotOrder[i]}").transform;
                newSlotTr.SetParent(slotsParent);

                // Place slot at its world position using European order mapping
                newSlotTr.position = GetWorldSlotPosition(SlotOrder[i]);
                _slotTransforms[i] = newSlotTr;
            }

            // Generate jump points (used for ball's jump animation path)
            var jumpPoints = JumpPointGenerationHelper.Generate(new JumpPointInitializeData
            {
                wheelTransform = wheelTransform,
                wheelCenter = WheelCenter,
                slotCount = 60,
                ringCount = 5,
                minRadius = 2.77f,
                maxRadius = 4f
            });

            // Parent object for jump positions
            var jumpPosParent = new GameObject("JumpPositions").transform;
            jumpPosParent.SetParent(wheelTransform);

            // Create transforms at each jump point for later use in jump animation
            foreach (Vector3 position in jumpPoints)
            {
                var jumpPosTransform = new GameObject("pos").transform;
                jumpPosTransform.SetParent(jumpPosParent);
                jumpPosTransform.position = position;
                _jumpPointTransforms.Add(jumpPosTransform);
            }
            
            ballController.Initialize(ball,ball.position, spinSpeed, wheelTransform, diamonds,_jumpPointTransforms,SlotOrder.Length);
            ballController.IdleRoutine();
            SetRandomDeterminationNumber();
        }

        public abstract RouletteType RouletteType { get; }

        public void Spin()
        {
            var targetPocket = FindPocketByNumber(_predeterminedNumber);
            if (targetPocket == null)
            {
                Debug.LogError($"Pocket for number {_predeterminedNumber} not found!");
                return;
            }

            ballController.SpinToTarget(targetPocket, _predeterminedNumber);
        }

        private void FixedUpdate()
        {
            wheelTransform.transform.Rotate(_axis * wheelRotateSpeed);
        }
        
        /// <summary>
        /// Converts a slot number to its world position on the wheel surface
        /// </summary>
        /// <param name="number">Roulette number</param>
        /// <returns>World position vector3</returns>
        private Vector3 GetWorldSlotPosition(int number)
        {
            // Find index of this number in the European order array to get its slot index
            var index = System.Array.IndexOf(SlotOrder, number);

            // Calculate the angle of this slot relative to the wheel
            var slotAngle = (360f - 37f * dividerAngle) / 37f;
            var localAngleDeg = index * (slotAngle + dividerAngle) + slotAngle / 2f;

            // Adjust for wheel's current rotation
            var worldAngleDeg = localAngleDeg - wheelTransform.eulerAngles.y;
            var angleRad = worldAngleDeg * Mathf.Deg2Rad;

            // Calculate world position on XZ plane using polar coordinates
            var x = WheelCenter.x + wheelTransform.position.x + slotPosWorldRadius * Mathf.Cos(angleRad);
            var z = WheelCenter.z + wheelTransform.position.z + slotPosWorldRadius * Mathf.Sin(angleRad);

            return new Vector3(x, WheelCenter.y, z);
        }

        /// <summary>
        /// Finds the Transform of the pocket corresponding to a roulette number
        /// </summary>
        /// <param name="number">Pocket number</param>
        /// <returns>Transform of pocket</returns>
        private Transform FindPocketByNumber(int number)
        {
            // The pocket number must be mapped to the slot index via _europeanOrder
            var index = System.Array.IndexOf(SlotOrder, number);
            if (index >= 0) return _slotTransforms[index];
            Debug.LogWarning($"Number {number} not found in European order array!");
            return null;
        }
        
        public WheelSaveData CaptureState()
        {
            return new WheelSaveData
            {
                lastDeterminedNumber = _predeterminedNumber
            };
        }

        public void RestoreState(WheelSaveData state)
        {
            _predeterminedNumber = state.lastDeterminedNumber;
            EventBus<DeterminedNumberSelectedEvent>.Raise(new DeterminedNumberSelectedEvent
            {
                SelectedNumber = _predeterminedNumber
            });
        }
    }
    [Serializable]
    public class WheelSaveData
    {
        public int lastDeterminedNumber;
    }
}