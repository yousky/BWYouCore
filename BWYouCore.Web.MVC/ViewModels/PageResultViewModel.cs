using System;
using System.Collections.Generic;
using System.Text;
using X.PagedList;

namespace BWYouCore.Web.MVC.ViewModels
{
    /// <summary>
    /// 페이지 정보 저장용 클래스
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResultViewModel<T>
    {
        /// <summary>
        /// 페이지 결과 객체
        /// </summary>
        public IEnumerable<T> Result { get; set; }
        /// <summary>
        /// 페이지 메타 정보
        /// </summary>
        public MetaData MetaData { get; set; }
        /// <summary>
        /// 기본 생성자
        /// </summary>
        public PageResultViewModel()
        {

        }
        /// <summary>
        /// 결과 객체 저장용 생성자. 메타 정보는 직접 주입 필요.
        /// </summary>
        /// <param name="result"></param>
        public PageResultViewModel(IEnumerable<T> result)
        {
            this.Result = result;
        }
        /// <summary>
        /// 결과 객체와 메타 정보를 한번에 저장하기 위한 생성자
        /// </summary>
        /// <param name="result"></param>
        /// <param name="MetaData"></param>
        public PageResultViewModel(IEnumerable<T> result, MetaData MetaData)
        {
            this.Result = result;
            this.MetaData = MetaData;
        }
        /// <summary>
        /// 페이지 결과 객체용 생성자. 가장 일반적으로 사용
        /// </summary>
        /// <param name="result"></param>
        public PageResultViewModel(IPagedList<T> result)
        {
            this.Result = result;
            this.MetaData = new MetaData(result);
        }
        /// <summary>
        /// IPagedList로 변환
        /// </summary>
        /// <returns></returns>
        public IPagedList<T> ToPagedList()
        {
            var p = new StaticPagedList<T>(this.Result, this.MetaData.PageIndex, this.MetaData.PageSize, (int)this.MetaData.TotalItemCount);    //long 값 존재 시 문제 될 것임.
            return p;
        }
    }
}
