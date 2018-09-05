using BWYouCore.Web.M.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.BindingModels
{
    public interface IBindingModel<TEntity>
        where TEntity : IDbModel
    {
        TEntity CreateBaseModel(TEntity src);
    }
}
