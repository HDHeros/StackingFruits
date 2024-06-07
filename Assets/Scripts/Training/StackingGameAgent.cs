using System;
using Gameplay.Blocks;
using Gameplay.GameCore;
using GameStructConfigs;
using HDH.UnityExt.Extensions;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Training
{
    public class StackingGameAgent : Agent
    {
        [SerializeField] private LevelConfig[] _levels;
        private StackingGame _game;
        private EnvironmentParameters _defaultParameters;
        
        public override void Initialize()
        {
            _game = new StackingGame();
            _defaultParameters = Academy.Instance.EnvironmentParameters;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(_game.LevelData.FieldSize);
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    sensor.AddObservation(GetCellValueSafe(x, y));
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            Vector2Int from = new Vector2Int(actions.DiscreteActions[0], actions.DiscreteActions[1]);
            Vector2Int to = new Vector2Int(actions.DiscreteActions[2], actions.DiscreteActions[3]);

            if (from == to)
            {
                SetReward(-1000);
                // EndEpisode();
                Log("FTE");
                return;
            }
            
            var enumerator = _game.MoveBlock(from, to);
            // Debug.Log($"Action received: from {from} to {to}");
            
            while (enumerator.MoveNext())
            {
                switch (enumerator.Current.Type)
                {
                    case GameEventType.BlockMovedByUser:
                        SetReward(-0.5f);
                        break;
                    case GameEventType.BlocksFell:
                        break;
                    case GameEventType.StackPerformed:
                        SetReward(5f);
                        Log("STACKED!");
                        break;
                    case GameEventType.GameWon:
                        SetReward(50);
                        EndEpisode();
                        Log("WIN!");
                        return;
                    case GameEventType.GameLost:
                        SetReward(-50f);
                        EndEpisode();
                        Log("LOOSE!");
                        return;
                    case GameEventType.WrongActionRequested:
                        Log("WAR!");
                        SetReward(-200f);
                        // EndEpisode();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            
        }

        public override void OnEpisodeBegin()
        {
            _game.Reinitialize(_levels.GetRandom().GetLevelData());
        }

        private int GetCellValueSafe(int x, int y)
        {
            if (_game.IsInRange(x, y))
                return (int) _game.GetCellValue(x, y).Type;
            return (int)BlockType.Blocker;
        }

        private void Log(string str)
        {
            Debug.Log(str);
        }

        private void OnDrawGizmosSelected()
        {
            for (int x = 0; x < 5; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    Gizmos.color = GetColorByCellValue(x, y);
                    Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
                }
            }
        }

        private Color GetColorByCellValue(int x, int y)
        {
            var type = _game.IsInRange(x, y) ? _game.GetCellValue(x, y).Type : BlockType.Blocker;
            
            switch (type)
            {
                case BlockType.Empty:
                    break;
                case BlockType.DragonfruitPink:
                    return Color.magenta;
                case BlockType.CoconutGreen:
                    return Color.green;
                case BlockType.Apricot:
                    return Color.yellow;
                case BlockType.CoconutHalf:
                    return Color.white;
                case BlockType.Fig_Blue:
                    return Color.blue;
                case BlockType.Fig_Green:
                    return Color.green;
                case BlockType.Fig_Yellow:
                    return Color.yellow;
                case BlockType.Grapefruit:
                    return Color.yellow;
                case BlockType.Guava_Green:
                    return Color.green;
                case BlockType.Kiwi_Half:
                    return Color.green;
                case BlockType.Lemon:
                    return Color.yellow;
                case BlockType.Lime:
                    return Color.green;
                case BlockType.Mango_Red:
                    return Color.red;
                case BlockType.Peach:
                    return Color.yellow;
                case BlockType.Persimmon_Yellow:
                    return Color.yellow;
                case BlockType.Pomegranate:
                    return Color.red;
                case BlockType.PricklyPear_Green:
                    return Color.green;
                case BlockType.PricklyPear_Orange:
                    return Color.red;
                case BlockType.PricklyPear_Pink:
                    return Color.magenta;
                    
                case BlockType.Blocker:
                    return Color.gray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            return Color.gray;
        }
    }
}