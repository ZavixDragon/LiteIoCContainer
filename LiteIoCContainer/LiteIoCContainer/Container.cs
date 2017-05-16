using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LiteIoCContainer
{
    public class Container
    {
        private readonly Dictionary<Type, Func<object>> _registeredTypes = new Dictionary<Type, Func<object>>();

        public void Register<T>(Type type)
        {
            var registerType = typeof(T);
            ValidateRegistration(registerType, type);
            _registeredTypes.Add(registerType, () => Construct(type));
        }

        public void RegisterInstance<T>(T instance)
        {
            var registerType = typeof(T);
            ValidateRegistration(registerType, instance.GetType());
            _registeredTypes.Add(registerType, () => instance);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type type)
        {
            if (!_registeredTypes.ContainsKey(type))
                throw new InvalidOperationException($"Can't resolve type {type}, because it has not been registered yet.");
            return _registeredTypes[type]();
        }

        private void ValidateRegistration(Type registerType, Type resolveType)
        {
            if (_registeredTypes.ContainsKey(registerType))
                throw new InvalidOperationException($"Can't register type {resolveType} as {registerType}, because {registerType} has already been registered.");
            if (resolveType.GetTypeInfo().IsAbstract)
                throw new ArgumentException($"Can't register type {resolveType} as {registerType}, because {resolveType} is abstract and cannot be constructed.");
            if (!registerType.GetTypeInfo().IsAssignableFrom(resolveType.GetTypeInfo()))
                throw new ArgumentException($"Can't register type {resolveType} as {registerType}, because {resolveType} is not assignable from {registerType}.");
        }

        private object Construct(Type type)
        {
            return Activator.CreateInstance(type, GetParameters(type).ToArray());
        }

        private IEnumerable<object> GetParameters(Type type)
        {
            return GetParameterTypes(type).Select(x => Resolve(x));
        }

        private IEnumerable<Type> GetParameterTypes(Type type)
        {
            return GetConstructor(type).GetParameters().Select(x => x.ParameterType);
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            var viableConstructors = type.GetTypeInfo().DeclaredConstructors.Where(x => IsViableConstructor(x)).ToList();
            if (!viableConstructors.Any())
                throw new InvalidOperationException($"Can't resolve type {type}, because there was no constructor that had all it's argument types registered.");
            return viableConstructors
                .OrderByDescending(x => x.GetParameters().Count())
                .First();
        }

        private bool IsViableConstructor(ConstructorInfo info)
        {
            return info.GetParameters()
                .Select(x => x.ParameterType)
                .All(x => _registeredTypes.ContainsKey(x));
        }
    }
}
