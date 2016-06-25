using Institute.WebApi.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using System.Web.Http;
using Institute.WebApi.Infrastructure.Extensions;
using System.Net.Http;
using Institute.BizComponent.Repositories;
using Institute.Entities;

namespace Institute.WebApi.Infrastructure.Core
{
    public class DataRepositoryFactory : IDataRepositoryFactory
    {
        public IEntityBaseRepository<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntityBase, new()
        {
            return request.GetDataRepository<T>();
        }
    }

    public interface IDataRepositoryFactory
    {
        IEntityBaseRepository<T> GetDataRepository<T>(HttpRequestMessage request) where T : class, IEntityBase, new();
    }
}