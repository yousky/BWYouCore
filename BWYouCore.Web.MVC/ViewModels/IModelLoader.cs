using BWYouCore.Web.M.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BWYouCore.Web.MVC.ViewModels
{
    public interface IModelLoader<TEntity>
        where TEntity : IDbModel
    {
        void LoadModel(TEntity baseModel, int curDepth = 0, int targetDepth = 0, string sort = "Id");
        Task LoadModelAsync(TEntity baseModel, int curDepth = 0, int targetDepth = 0, string sort = "Id");
    }
}
