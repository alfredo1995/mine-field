using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Components
{
    public class Grid : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite disabled;
        [SerializeField] private Sprite hidden;
        [SerializeField] private Sprite diamond;
        [SerializeField] private Sprite mine;
        [SerializeField] private Sprite brokenCell;
        [SerializeField] private RuntimeAnimatorController glowBall, defaultBall, clickUnderlayer, defaultUnderlayer;
        public event Action<int> onSelectedCell;

        private List<Cell> _cells = new();

        public int Size => _cells.Count;

        private readonly List<int> _unselectsCells = new();

        /// <summary>
        /// Create grid with size, with default cell asset
        /// </summary>
        /// <param name="size">literal size of grid</param>
        public void CreateGrid(int size)
        {
            _cells = new List<Cell>(size);
            var cellAsset = Resources.LoadAsync<Cell>("Cell_[Button]").asset as Cell;
            
            for (var i = 0; i < size; i++)
            {
                var index = i;
                var cell = Instantiate(cellAsset, transform);

                cell.name = index.ToString();
                cell.SetBackground(hidden);
                cell.ShowBall(true);
                cell.SetInteractable(false);
                cell.CellButton.onClick.AddListener(() => OnSelectedCell(index));

                _cells.Add(cell);
                _unselectsCells.Add(index);
            }

            Resources.UnloadUnusedAssets();
        }

        private void OnSelectedCell(int index)
        {
            onSelectedCell?.Invoke(index);
            if (_unselectsCells.Contains(index))
                _unselectsCells.Remove(index);
            else _unselectsCells.Add(index);
        }
        
        public void DeselectCell(int index) => _unselectsCells.Add(index);

        public void UpdateGrid(List<int> grid, int deltaIndex = -1)
        {
            if (deltaIndex == -1)
            {
                for (var i = 0; i < grid.Count; i++)
                {
                    _cells[i].SetBackground(grid[i] == (int)CellType.Diamonds ? diamond : mine);
                    UpdateCell(i, grid[i]);
                }
            }
            else
            {
                if (grid[deltaIndex] == (int)CellType.Hidden) return;
                _cells[deltaIndex].SetBackground(diamond);
                _cells[deltaIndex].ShowUnderlay(grid[deltaIndex] != (int)CellType.Hidden);
                UpdateCell(deltaIndex, grid[deltaIndex]);
                _cells[deltaIndex].ShowBall(grid[deltaIndex] != (int)CellType.Hidden);
            }
        }

        private void UpdateCell(int index, int cellType)
        {
            _cells[index].SetUnderlayAnimation(cellType == (int)CellType.Diamonds ? clickUnderlayer : defaultUnderlayer);
            _cells[index].ShowUnderlay(cellType == (int)CellType.Hidden);
            _cells[index].SetUnderlayColor(cellType != (int)CellType.Mine ? Color.white : Color.red);
            _cells[index].SetBallAnimation(cellType == (int)CellType.Hidden ? glowBall : defaultBall);
            _cells[index].ShowBall(false);
            _cells[index].ShowOverlay(cellType == (int)CellType.Diamonds);
        }

        /// <summary>
        /// Enable or Disable raycast of all cells
        /// </summary>
        /// <param name="value"> true or false</param>
        public void SetInteractable(bool value)
        {
            foreach (var slot in _cells) slot.SetInteractable(value);
        }
        
        public void SetInteractableUnselectedCell(bool value = false)
        {
            var sprite = value ? hidden : disabled;
            
            foreach (var index in _unselectsCells)
            {
                _cells[index].ShowBall(value);
                _cells[index].SetBackground(sprite);
                _cells[index].SetInteractable(value);
            }
        }

        public void SetInteractableBackground(int cellID, bool interactable = false)
        {
            _cells[cellID].ShowBall(interactable);
            _cells[cellID].SetBackground(interactable ? hidden : disabled);
            _cells[cellID].SetInteractable(interactable);
        }
        
        /// <summary>
        /// Reset all cells to hidden state
        /// </summary>
        public void ResetGrid()
        {
            _unselectsCells.Clear();

            foreach (var cell in _cells)
            {
                cell.Explode(false);
                cell.SetBackgroundColor(Color.white);
                cell.SetBackground(hidden);
                
                cell.ShowUnderlay(false);
                cell.ShowUnderlay(true);
                
                cell.SetUnderlayColor(Color.white);
                
                cell.ShowBall(false);
                cell.ShowBall(true);
                cell.ShowOverlay(false);

                cell.SetBallAnimation(glowBall);
                cell.SetUnderlayAnimation(defaultUnderlayer);

                cell.IsSelected = false;
                _unselectsCells.Add(_cells.IndexOf(cell));
            }
        }

        public void ResetUnselectedCells()
        {
            foreach (var index in _unselectsCells)
            {
                _cells[index].SetBackgroundColor(Color.white);
                _cells[index].SetBackground(hidden);
                _cells[index].SetUnderlayAnimation(defaultUnderlayer);
                _cells[index].ShowUnderlay(false);
                _cells[index].ShowUnderlay(true);
                _cells[index].SetUnderlayColor(Color.white);
                _cells[index].SetBallAnimation(glowBall);
                _cells[index].ShowBall(false);
                _cells[index].ShowBall(true);
                _cells[index].ShowOverlay(false);
                _cells[index].IsSelected = false;
            }
        }

        /// <summary>
        /// Click in cell to select
        /// </summary>
        /// <param name="index"></param>
        public void SelectCell(int index)
        {
            _cells[index].CellButton.onClick.Invoke();
        }

        public void ApplyCellImage(int index, Sprite sprite, Color newBallColor)
        {
            _cells[index].SetBackground(sprite);
            _cells[index].SetBallColor(newBallColor);
        }

        public void ApplyCellColor(int index, Color color)
        {
            _cells[index].SetUnderlayColor(color);
            _cells[index].SetBackgroundColor(color);
            _cells[index].SetBallColor(color);
        }

        public void BlastAnimation(int index, List<int> cells, Action OnDone = null)
        {
            _cells[index].SetBackgroundColor(default);
            _cells[index].ShowBall(false);
            _cells[index].Explode(true);
            _cells[index].SetBackground(brokenCell);

            //get neighbors not getting all of them
            var affectedCells = GridHelper.GetNeighbors(index, 5);
            const float delay = .9f;
            StartCoroutine(DelayedUpdate(delay, cells, OnDone));
            for (var i = 0; i < affectedCells.Count; i++)
            {
                StartCoroutine(ScriptAnimations.Blast(_cells[affectedCells[i]],
                    GridHelper.GetDirection(affectedCells[i], index, 5), delay));
            }
        }

        private IEnumerator DelayedUpdate(float value,List<int> cells,Action onDone = null)
        {
            yield return new WaitForSeconds(value);
            UpdateGrid(cells);
            onDone?.Invoke();
        }
    }

    public enum CellType
    {
        Hidden = 0,
        Diamonds = 1,
        Mine = 2
    }
}