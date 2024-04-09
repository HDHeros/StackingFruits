using Gameplay.Blocks;
using Gameplay.GameCore;
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
        public BlockView EmptyBlock;
        public Vector2Int FieldSize;
        public BlockView[] Blocks;
        public LevelPreview LevelPreview;
        [TableMatrix(RowHeight = 60, DrawElementMethod = "DrawCell"), ShowInInspector]
        private BlockView[,] Blocks2D;

        public LevelData<BlockView> GetLevelData()
        {
            return new LevelData<BlockView>()
            {
                Blocks = Blocks,
                EmptyBlockValue = EmptyBlock,
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
                    Blocks[y * FieldSize.x + x] = Blocks2D[x, modY];
                    if (Blocks2D[x, modY] == null)
                        Blocks2D[x, modY] = EmptyBlock;
                }
            }
            
            EditorUtility.SetDirty(this);
        }

        [Button]
        private void LoadMatrixFromArray()
        {
            Blocks2D = new BlockView[FieldSize.x, FieldSize.y];
            for (var i = 0; i < Blocks.Length; i++)
            {
                BlockView block = Blocks[i];
                int x = i % FieldSize.x;
                int y = FieldSize.y - 1 - Mathf.FloorToInt((float) i / FieldSize.x);
                Blocks2D[x, y] = block;
            }
        }
        
        [Button]
        private void ResetMatrix()
        {
            Blocks2D = new BlockView[FieldSize.x, FieldSize.y];
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
            
            return view;
        }
    }
}