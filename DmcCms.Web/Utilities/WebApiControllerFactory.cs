using Dmc.Cms.App;
using Dmc.Cms.Model;
using Dmc.Core.DI;
using Dmc.Identity;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Dmc.Cms.Web
{
    public class WebApiControllerFactory : IHttpControllerActivator
    {
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(IService), typeof(IUnitOfWork), typeof(IIdentityUnitOfWork<User>) };
        private readonly DependencyInjectionContainer _Container;
        private readonly DefaultHttpControllerActivator _DefaultControllerActivator;

        public WebApiControllerFactory(DependencyInjectionContainer container)
        {
            _DefaultControllerActivator = new DefaultHttpControllerActivator();
            _Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var constructors = controllerType.GetConstructors();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (!parameters.All(o => (_SupportedTypes.Any(s => s.IsAssignableFrom(o.ParameterType)))))
                {
                    continue;
                }

                Guid requestId = Guid.NewGuid();
                List<object> resolved = new List<object>();

                foreach (var item in parameters)
                {
                    resolved.Add(_Container.Resolve(item.ParameterType, requestId)); // for each resolve during request id must be the same. this is crap.
                }

                return (IHttpController)constructor.Invoke(resolved.ToArray());
            }

            return _DefaultControllerActivator.Create(request, controllerDescriptor, controllerType);
        }
    }
}