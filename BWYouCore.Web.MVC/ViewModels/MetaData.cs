using System;
using System.Collections.Generic;
using System.Text;
using X.PagedList;

namespace BWYouCore.Web.MVC.ViewModels
{
    public class MetaData
    {
        #region PageInfo

        public long TotalItemCount { get; set; }
        public int TotalPageCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool IsFirstPage { get; set; }
        public bool IsLastPage { get; set; }

        #endregion PageInfo

        public MetaData()
        {

        }
        public MetaData(IPagedList pagedList)
        {
            this.TotalItemCount = pagedList.TotalItemCount;
            this.TotalPageCount = pagedList.PageCount;
            this.PageIndex = pagedList.PageNumber;
            this.PageSize = pagedList.PageSize;
            this.HasNextPage = pagedList.HasNextPage;
            this.HasPreviousPage = pagedList.HasPreviousPage;
            this.IsFirstPage = pagedList.IsFirstPage;
            this.IsLastPage = pagedList.IsLastPage;
        }
        public MetaData(int totalItemCount, int pageIndex, int pageSize)
        {
            this.TotalItemCount = totalItemCount;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;

            this.TotalPageCount = TotalItemCount > 0 ? (int)Math.Ceiling(TotalItemCount / (double)PageSize) : 0;
            this.HasNextPage = PageIndex < TotalPageCount;
            this.HasPreviousPage = PageIndex > 1;
            this.IsFirstPage = PageIndex == 1;
            this.IsLastPage = PageIndex >= TotalPageCount;
        }

    }
}
