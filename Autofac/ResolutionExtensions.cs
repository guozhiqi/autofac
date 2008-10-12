﻿using System;
using System.Collections.Generic;
using Autofac.Services;
using Autofac.Registry;

namespace Autofac
{
    public static class ResolutionExtensions
    {
        static readonly IEnumerable<Parameter> NoParameters = new Parameter[0];

        public static T Resolve<T>(this IComponentContext context, Guid registrationId)
        {
            return Resolve<T>(context, registrationId, NoParameters);
        }

        public static T Resolve<T>(this IComponentContext context, Guid registrationId, IEnumerable<Parameter> parameters)
        {
            return (T)Resolve(context, new UniqueService(registrationId), parameters);
        }

        public static T Resolve<T>(this IComponentContext context, string name)
        {
            return Resolve<T>(context, name, NoParameters);
        }

        public static T Resolve<T>(this IComponentContext context, string name, IEnumerable<Parameter> parameters)
        {
            return (T)Resolve(context, new NamedService(name), parameters);
        }

        public static T Resolve<T>(this IComponentContext context)
        {
            return Resolve<T>(context, NoParameters);
        }

        public static T Resolve<T>(this IComponentContext context, IEnumerable<Parameter> parameters)
        {
            return (T)Resolve(context, typeof(T), parameters);
        }

        public static object Resolve(this IComponentContext context, Type serviceType)
        {
            return Resolve(context, serviceType, NoParameters);
        }

        public static object Resolve(this IComponentContext context, Type serviceType, IEnumerable<Parameter> parameters)
        {
            return Resolve(context, new TypedService(serviceType), parameters);
        }

        public static object Resolve(this IComponentContext context, string serviceName)
        {
            return Resolve(context, serviceName, NoParameters);
        }

        public static object Resolve(this IComponentContext context, string serviceName, IEnumerable<Parameter> parameters)
        {
            return Resolve(context, new NamedService(serviceName), parameters);
        }

        public static object Resolve(this IComponentContext context, Service service)
        {
            return Resolve(context, service, NoParameters);
        }

        public static object Resolve(this IComponentContext context, Service service, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(parameters, "parameters");

            object instance;
            var successful = context.TryResolve(service, parameters, out instance);
            if (!successful)
                throw new ComponentNotRegisteredException(service);
            return instance;
        }

        public static T ResolveOptional<T>(this IComponentContext context)
            where T : class
        {
            return ResolveOptional<T>(context, NoParameters);
        }

        public static T ResolveOptional<T>(this IComponentContext context, IEnumerable<Parameter> parameters)
            where T : class
        {
            return (T)ResolveOptional(context, new TypedService(typeof(T)), parameters);
        }

        public static T ResolveOptional<T>(this IComponentContext context, string serviceName)
            where T : class
        {
            return ResolveOptional<T>(context, serviceName, NoParameters);
        }

        public static T ResolveOptional<T>(this IComponentContext context, string serviceName, IEnumerable<Parameter> parameters)
            where T : class
        {
            return (T)ResolveOptional(context, new NamedService(serviceName), parameters);
        }

        public static object ResolveOptional(this IComponentContext context, Service service)
        {
            return ResolveOptional(context, service, NoParameters);
        }

        public static object ResolveOptional(this IComponentContext context, Service service, IEnumerable<Parameter> parameters)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(service, "service");
            Enforce.ArgumentNotNull(parameters, "parameters");

            object instance;
            context.TryResolve(service, parameters, out instance);
            return instance;
        }

        public static bool IsRegistered<T>(this IComponentContext context)
        {
            return IsRegistered(context, typeof(T));
        }

        public static bool IsRegistered(this IComponentContext context, Type serviceType)
        {
            return IsRegistered(context, new TypedService(serviceType));
        }

        public static bool IsRegistered(this IComponentContext context, string serviceName)
        {
            return IsRegistered(context, new NamedService(serviceName));
        }

        public static bool IsRegistered(this IComponentContext context, Service service)
        {
            Enforce.ArgumentNotNull(context, "context");
            Enforce.ArgumentNotNull(service, "service");

            return context.ComponentRegistry.IsRegistered(service);
        }

        public static bool TryResolve(this IComponentContext context, Service service, IEnumerable<Parameter> parameters, out object instance)
        {
            IComponentRegistration registration;
            if (!context.ComponentRegistry.TryGetRegistration(service, out registration))
            {
                instance = null;
                return false;
            }

            instance = context.Resolve(registration, parameters);
            return true;
        }

        public static bool TryResolve(this IComponentContext context, Type type, out object instance)
        {
            Enforce.ArgumentNotNull(context, "context");
            return context.TryResolve(new TypedService(type), NoParameters, out instance);
        }

        public static bool TryResolve(this IComponentContext context, string name, out object instance)
        {
            Enforce.ArgumentNotNull(context, "context");
            return context.TryResolve(new NamedService(name), NoParameters, out instance);
        }
    }
}