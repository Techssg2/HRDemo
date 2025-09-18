using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Unity;

namespace Aeon.Navigation
{
    public class UnityFilterProvider : ActionDescriptorFilterProvider, IFilterProvider
    {
        private IUnityContainer _container;

        public UnityFilterProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<FilterInfo> GetFilters(HttpConfiguration configuration, HttpActionDescriptor actionDescriptor)
        {
            var attributes = base.GetFilters(configuration, actionDescriptor);

            foreach (var attr in attributes)
            {
                _container.BuildUp(attr.Instance.GetType(), attr.Instance);
            }
            return attributes;
        }
    }
}