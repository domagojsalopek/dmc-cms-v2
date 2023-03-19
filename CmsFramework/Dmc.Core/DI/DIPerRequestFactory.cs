using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Core.DI
{
    internal class DIPerRequestFactory
    {
        #region Fields

        private readonly IList<DependencyInjectionRegisteredObject> _RegisteredObjects;
        private Dictionary<string, object> _AlreadyResolved = new Dictionary<string, object>();

        #endregion

        #region Constructors

        internal DIPerRequestFactory(IList<DependencyInjectionRegisteredObject> registeredObjects)
        {
            _RegisteredObjects = registeredObjects;
        }

        #endregion

        #region Methods

        public object Resolve(Type typeToResolve)
        {
            string key = typeToResolve.FullName;

            if (_AlreadyResolved.ContainsKey(key))
            {
                return _AlreadyResolved[key];
            }

            var registeredObject = _RegisteredObjects
                .FirstOrDefault(o => o.TypeToResolve == typeToResolve);

            if (registeredObject == null)
            {
                throw new InvalidOperationException(string.Format("The type {0} has not been registered", typeToResolve.Name));
            }

            var objectResult = GetInstance(registeredObject);

            _AlreadyResolved[key] = objectResult;

            return objectResult;
        }

        #endregion

        #region Private Methods

        private object GetInstance(DependencyInjectionRegisteredObject registeredObject)
        {
            var parameters = ResolveConstructorParameters(registeredObject);

            return registeredObject.CreateInstance(parameters);
        }

        private object[] ResolveConstructorParameters(DependencyInjectionRegisteredObject registeredObject)
        {
            var constructorInfo = registeredObject
                .ConcreteType
                .GetConstructors()
                .First();

            List<object> resolvedParameters = new List<object>();

            foreach (var parameter in constructorInfo.GetParameters())
            {
                var injectedParameter = Resolve(parameter.ParameterType); // which takes a look at it's parameters

                resolvedParameters.Add(injectedParameter);
            }

            return resolvedParameters.ToArray();
        }

        #endregion
    }
}
