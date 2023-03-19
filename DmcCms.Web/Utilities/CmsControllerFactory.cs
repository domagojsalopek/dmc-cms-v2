using Dmc.Cms.App;
using Dmc.Cms.App.Services;
using Dmc.Cms.Model;
using Dmc.Cms.Web.Settings;
using Dmc.Core.DI;
using Dmc.Identity;
using Dmc.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Dmc.Cms.Web
{
    public class CmsControllerFactory : DefaultControllerFactory
    {
        #region Private Fields

        private readonly DependencyInjectionContainer _Container;
        private static readonly Type[] _SupportedTypes = new Type[] { typeof(IService), typeof(IIdentityUnitOfWork<User>), typeof(IUnitOfWork), typeof(IAppConfig), typeof(ICmsUnitOfWorkFactory) };

        #endregion

        #region Constructors

        //TODO !!! CACHE!!!!!

        public CmsControllerFactory(DependencyInjectionContainer container)
        {
            _Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        #endregion

        #region Overrides

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null) // can this happen?
            {
                return null;
                //return base.GetControllerInstance(requestContext, controllerType);
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
            {
                throw new ArgumentException(string.Format("Type requested is not a controller: {0}", controllerType.Name), nameof(controllerType));
            }

            var constructors = controllerType.GetConstructors();

            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if (!parameters.All(o => (_SupportedTypes.Any(s => s.IsAssignableFrom(o.ParameterType)))))
                {
                    continue;
                }

                // first matched where all assignable
                Guid requestId = Guid.NewGuid();

                // eagerly prepare
                _Container.BeginRequest(requestId);

                // start
                List<object> resolved = new List<object>();

                foreach (var item in parameters)
                {
                    if (item.ParameterType == typeof(IAppConfig))
                    {
                        resolved.Add(AppConfig.Instance); // quick & dirty
                        continue;
                    }

                    resolved.Add(_Container.Resolve(item.ParameterType, requestId)); // for each resolve during request id must be the same. this is crap.
                }

                return (IController)constructor.Invoke(resolved.ToArray());
            }

            return base.GetControllerInstance(requestContext, controllerType);
        }

        #endregion
    }
}