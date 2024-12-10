using System.Collections.Generic;
using UnityEngine;

namespace UI.Components
{
    public class GridHelper : MonoBehaviour
    {
        /// <summary>
        /// 
        /// Vizinhança de Moore
        /// Grafos de Moore
        /// </summary>
        /// <param name="index">the index reference for the neighbors</param>
        /// <param name="width">the width of the grid</param>
        /// <param name="size">the maximum distance from index is counted as a neighbor</param>
        /// <returns></returns>
        public static List<int> GetNeighbors(int index, int width, int size = 1)
        {
            //the returning list
            var neighbors = new List<int>();
            //the distance from each axis you need to get. Ex: 3,5,7...
            var neihborsDistance = 1 + size * 2;
            //the total area needed to be checked. Ex: 3x3,5x5,7x7....
            var totalCells = neihborsDistance * neihborsDistance;
            //the total grid size
            var gridSize = width * width;

            for (var i = 0; i < totalCells; i++)
            {
                //getting the values of distance from index for each axis
                var x = i % neihborsDistance - size;
                var y = i / neihborsDistance % neihborsDistance - size;
                //checking if the value is within the grid before assigning it as a neighbor
                var value = width * (index / width + y) + Mathf.Clamp(index % width + x, 0, width - 1);
                if (value >= 0 && value < gridSize && value != index) neighbors.Add(value);
            }
            return neighbors;
        }

        /// <summary>
        /// returns the direction target index is from the reference
        /// </summary>
        /// <param name="index">the index you want to know the direction for</param>
        /// <param name="reference">the index reference for the target</param>
        /// <param name="gridWidth">the width of the grid</param>
        /// <returns>the direction target index is from the reference</returns>
        public static Vector2Int GetDirection(int index, int reference, int gridWidth)
        {
            var returnValue = Vector2Int.zero;
            //getting the values of distance from index for each axis
            returnValue.x = index % gridWidth - reference % gridWidth;
            returnValue.y = reference / gridWidth - index / gridWidth;

            return returnValue;
        }

    }
}