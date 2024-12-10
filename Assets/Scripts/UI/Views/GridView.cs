using Pattern;
using UnityEngine;
using Grid = UI.Components.Grid;

namespace UI
{
    public class GridView : MonoBehaviour, IViewState
    {
        [Header("Components")]
        [SerializeField] private Grid grid;
        
        public int Size => grid.Size;

        public void CreateGrid(int size)
        {
            grid.CreateGrid(size);
        }

        public void ClearGrid()
        {
            grid.ResetGrid();
        }

        #region IViewState

        public void Lock()
        {
            grid.SetInteractable(false);
        }

        public void Unlock()
        {
            grid.SetInteractable(true);
        }

        #endregion
        
    }
}