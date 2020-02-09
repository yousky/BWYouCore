using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BWYouCore.Web.MVC.ViewModels
{
    public class CursorMetaData<TEntity>
    {
        #region CursorInfo

        public Object Before { get; set; }
        public Object After { get; set; }

        /// <summary>
        /// 검색 범위에 해당하는 전체 개수
        /// </summary>
        public long TotalUnlimitItemCount { get; set; }

        public int Limit { get; set; }

        public bool IsDescending { get; set; }
        public bool IsRemaining { get; set; }

        #endregion CursorInfo

        public CursorMetaData()
        {

        }
        public CursorMetaData(IEnumerable<TEntity> result, string sortOrder, int limit, long totalUnlimitItemCount)
        {
            this.IsDescending = false;
            string cursorProp = sortOrder;
            if (sortOrder.StartsWith("-") == true)
            {
                this.IsDescending = true;
                cursorProp = sortOrder.Substring(1);
            }

            if (result.LongCount() > 0)
            {
                if (this.IsDescending == true)
                {
                    //내림차순
                    //3,2,1 일 경우 after : 3, before 1
                    //프론트에서 내림차순일 경우 before에 1만 넣어서 GetFilteredListAsync 호출하면 1보다 작은 것을 검색 해서 준다.
                    this.Before = GetPropertyValue(result.LastOrDefault(), cursorProp);
                    this.After = GetPropertyValue(result.FirstOrDefault(), cursorProp);
                }
                else
                {
                    //오름차순
                    //1,2,3 일 경우 after : 3, before 1
                    //프론트에서 오름차순일 경우 after에 3만 넣어서 GetFilteredListAsync 호출하면 3보다 큰 것을 검색 해서 준다.
                    this.After = GetPropertyValue(result.LastOrDefault(), cursorProp);
                    this.Before = GetPropertyValue(result.FirstOrDefault(), cursorProp);
                }
            }

            this.TotalUnlimitItemCount = totalUnlimitItemCount;
            this.Limit = limit;
            this.IsRemaining = totalUnlimitItemCount > result.LongCount();
        }

        private Object GetPropertyValue(TEntity source, string propertyName)
        {
            return source.GetType().GetProperty(propertyName).GetValue(source, null);
        }

    }
}
