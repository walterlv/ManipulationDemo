using System;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace ManipulationDemo
{
    public static class Refin
    {
        public static TypeRefin TypeOf(this Assembly assembly, string name)
        {
            return assembly.GetTypes().FirstOrDefault(x => x.Name == name);
        }

        public static T Get<T>(this TypeRefin typeRefin, string propertyOrFieldName)
        {
            var type = typeRefin.Type;
            var propertyInfo = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(x => x.Name == propertyOrFieldName);
            if (propertyInfo != null)
            {
                return (T)propertyInfo.GetValue(null);
            }
            var fieldInfo = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                .FirstOrDefault(x => x.Name == propertyOrFieldName);
            if (fieldInfo != null)
            {
                return (T)fieldInfo.GetValue(null);
            }
            return default(T);
        }
    }

    public class TypeRefin
    {
        public static readonly TypeRefin Null = new TypeRefin();

        public TypeRefin(Type type = null)
        {
            Type = type;
        }

        public Type Type { get; }

        public static implicit operator TypeRefin(Type type)
        {
            return new TypeRefin(type);
        }

        public static implicit operator Type(TypeRefin typeRefin)
        {
            return typeRefin.Type;
        }
    }
}
