using BWYouCore.Web.M.Models;
using BWYouCore.Web.MVC.DAOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BWYouCore.Web.MVC.Services
{
    public class BWStringEntityService<TEntity> : BWEntityService<TEntity, string>
        where TEntity : BWModel<string>
    {
        public BWStringEntityService(DbContext dbContext)
            : base(dbContext)
        {

        }

        public BWStringEntityService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

    }
}
