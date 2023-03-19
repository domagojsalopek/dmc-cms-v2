using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dmc.Core.DI
{
    public class DependencyInjectionRegisteredObject
    {
        #region Constructors

        public DependencyInjectionRegisteredObject(Type typeToResolve, Type concreteType)
        {
            TypeToResolve = typeToResolve;
            ConcreteType = concreteType;
        }

        #endregion

        #region Properties

        public Type TypeToResolve
        {
            get;
            private set;
        }

        public Type ConcreteType
        {
            get;
            private set;
        }

        //public object Instance
        //{
        //    get;
        //    private set;
        //}

        #endregion

        #region Public Methods

        public object CreateInstance(object[] args)
        {
            if (args == null || args.Length <= 0)
            {
                return Activator.CreateInstance(ConcreteType);
            }

            return Activator.CreateInstance(ConcreteType, args);
        }

        #endregion
    }
}
