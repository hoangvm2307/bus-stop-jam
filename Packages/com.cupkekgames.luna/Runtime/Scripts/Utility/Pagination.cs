
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CupkekGames.Luna
{
    [Serializable]
    public class Pagination<T>
    {
        public List<T> Data;
        public int CurrentPage;
        public int ItemsPerPage;
        public int TotalPages => Mathf.Max(1, Mathf.CeilToInt((float)Data.Count / ItemsPerPage));
        public event Action<int> OnPageChange;

        public Pagination(List<T> data, int itemsPerPage)
        {
            Data = data;
            ItemsPerPage = itemsPerPage;

            if (ItemsPerPage <= 0)
            {
                Debug.LogError("ItemsPerPage must be greater than zero. Please set a valid value.");
            }
        }

        public bool NextPage()
        {
            if (CurrentPage < TotalPages - 1)
            {
                CurrentPage++;
                OnPageChange?.Invoke(CurrentPage);
                return true;
            }
            return false;
        }

        public bool PreviousPage()
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
                OnPageChange?.Invoke(CurrentPage);
                return true;
            }
            return false;
        }

        public bool GoToPage(int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < TotalPages)
            {
                CurrentPage = pageIndex;
                OnPageChange?.Invoke(CurrentPage);
                return true;
            }
            return false;
        }

        public List<T> GetCurrentPageElements()
        {
            var pageElements = new List<T>();

            // Calculate the starting index and number of elements on the current page
            int startIndex = CurrentPage * ItemsPerPage;
            int count = Math.Min(ItemsPerPage, Data.Count - startIndex);

            for (int i = 0; i < count; i++)
            {
                pageElements.Add(Data[startIndex + i]);
            }

            return pageElements;
        }

        public int GetStartIndex(int page)
        {
            return page * ItemsPerPage;
        }
    }
}