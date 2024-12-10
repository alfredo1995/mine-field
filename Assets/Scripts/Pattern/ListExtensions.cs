using System.Collections.Generic;

namespace Managers
{
    public static class ListExtensions
    {
        public static int GetDeltaIndex(this List<int> previousList, List<int> newList)
        {
            if(previousList.Count != newList.Count) return -1;

            for(var i = 0; i < previousList.Count; i++)
            {
                if(previousList[i] != newList[i]) return i;
            }

            return -1;
        }
    }
}