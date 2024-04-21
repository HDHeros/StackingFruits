using System.Linq;
using Gameplay.Blocks;
using Gameplay.GameCore;
using HDH.UnityExt.Extensions;
using Menu;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace GameStructConfigs
{
    [CreateAssetMenu(menuName = "Configs/Game/Level", fileName = "LevelConfig", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public string Id;
        public BlockView EmptyBlock;
        public Vector2Int FieldSize;
        public int InSectionViewIndex;
        public BlockView[] Blocks;
        public LevelPreview LevelPreview;
        [TableMatrix(RowHeight = 60), ShowInInspector]
        private GameObject[,] Blocks2D;

        public LevelData GetLevelData()
        {
            return new LevelData()
            {
                Blocks = Blocks.Select(b => b.GetBlockData()).ToArray(),
                EmptyBlockValue = EmptyBlock.GetBlockData(),
                FieldSize = FieldSize,
            };
        }

        [Button]
        private void SerializeMatrix()
        {
            Blocks = new BlockView[FieldSize.x * FieldSize.y];
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    int modY = FieldSize.y - y - 1;
                    Blocks[y * FieldSize.x + x] = Blocks2D[x, modY].GetComponent<BlockView>();
                    if (Blocks2D[x, modY] == null)
                        Blocks2D[x, modY] = EmptyBlock.gameObject;
                }
            }
            
            EditorUtility.SetDirty(this);
        }
        
        private void LoadMatrixFromArray()
        {
            Blocks2D = new GameObject[FieldSize.x, FieldSize.y];
            for (var i = 0; i < Blocks.Length; i++)
            {
                BlockView block = Blocks[i];
                int x = i % FieldSize.x;
                int y = FieldSize.y - 1 - Mathf.FloorToInt((float) i / FieldSize.x);
                Blocks2D[x, y] = block.IsNull() ? null : block.gameObject;
            }
        }

        private void OnValidate()
        {
            if (Blocks2D == null) return;
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    if (Blocks2D[x, y] == null && EmptyBlock.IsNotNull())
                        Blocks2D[x, y] = EmptyBlock.gameObject;
                }
            }
        }

        [Button]
        private void ResetMatrix()
        {
            Blocks2D = new GameObject[FieldSize.x, FieldSize.y];
        }

        [Button]
        private void SetIdByName()
        {
            Id = name;
            EditorUtility.SetDirty(this);
        }

        // ReSharper disable once UnusedMember.Local
        private static BlockView DrawCell(Rect rect, BlockView view)
        {
            if (DragAndDropUtilities.IsDragging &&
                rect.Contains(Event.current.mousePosition))
            {
                view = DragAndDropUtilities.DropZone(rect, view, false);
                DragAndDropUtilities.DrawDropZone(rect, view == null ? null : view.gameObject, 
                    new GUIContent(view == null ? Texture2D.blackTexture : AssetPreview.GetMiniTypeThumbnail(typeof(GameObject))), DragAndDropUtilities.CurrentDragId);
            }
            else
            {
                if (view != null )
                    EditorGUI.DrawPreviewTexture(rect, AssetPreview.GetAssetPreview(view.gameObject) ?? Texture2D.grayTexture, null, ScaleMode.ScaleToFit);
            }
            
            if (Event.current.type == EventType.MouseDrag)
            {
                DragAndDrop.PrepareStartDrag();

                // Set up what we want to drag
                DragAndDrop.paths = new[] { AssetDatabase.GetAssetPath(view) };

                // Start the actual drag
                DragAndDrop.StartDrag("Dragging title");

                // Make sure no one uses the event after us
                Event.current.Use();
            }
            
            return view;
        }

        private void OnEnable()
        {
            #if UNITY_EDITOR
            LoadMatrixFromArray();
            #endif
        }
    }
}