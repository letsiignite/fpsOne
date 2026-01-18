using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class UniqueRandom<T>
    {
        private List<T> items;
        private int index;

        public UniqueRandom(List<T> source)
        {
            items = new List<T>(source);
            Shuffle();
            index = 0;
        }

        public T GetNext()
        {
            if (index >= items.Count-1)
            {
                Shuffle();
                index = 0;
            }
            Debug.Log(" UniqueRandom Index = "+index);
            return items[index++];
        }

        private void Shuffle()
        {
            for (int i = 0; i < items.Count; i++)
            {
                int rand = Random.Range(i, items.Count);
                (items[i], items[rand]) = (items[rand], items[i]);
            }
        }
    }
}