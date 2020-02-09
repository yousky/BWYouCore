using BWYouCore.Web.MVC.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace BWYouCore.Web.MVC.ViewModels
{
    /// <summary>
    /// 커서 형태의 정보 저장용 클래스
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class CursorResultViewModel<TEntity>
    {
        /// <summary>
        /// 페이지 결과 객체
        /// </summary>
        public IEnumerable<TEntity> Result { get; set; }
        /// <summary>
        /// 페이지 메타 정보
        /// </summary>
        public CursorMetaData<TEntity> MetaData { get; set; }
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public CursorResultViewModel()
        {

        }
        /// <summary>
        /// 결과 객체 저장용 생성자. 메타 정보는 직접 주입 필요.
        /// </summary>
        /// <param name="result"></param>
        public CursorResultViewModel(IEnumerable<TEntity> result)
        {
            this.Result = result;
        }
        /// <summary>
        /// 결과 객체와 메타 정보를 한번에 저장하기 위한 생성자
        /// </summary>
        /// <param name="result"></param>
        /// <param name="MetaData"></param>
        public CursorResultViewModel(IEnumerable<TEntity> result, CursorMetaData<TEntity> MetaData)
        {
            this.Result = result;
            this.MetaData = MetaData;
        }
        public static async Task<CursorResultViewModel<TEntity>> BuildAsync(IQueryable<TEntity> query, string sortOrder, int limit)
        {
            var unlimitCnt = await query.LongCountAsync();
            var limitListResult = await query.SortBy(sortOrder).Take(limit).ToListAsync();
            var cmd = new CursorMetaData<TEntity>(limitListResult, sortOrder, limit, unlimitCnt);
            var crvm = new CursorResultViewModel<TEntity>(limitListResult, cmd);

            return crvm;
        }
    }
}
