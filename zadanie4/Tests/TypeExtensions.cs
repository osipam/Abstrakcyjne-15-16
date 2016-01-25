//Mateusz Osipa
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public static class TypeExtensions
    {
        public static string FriendlyName(this Type t)
        {
            if (t.IsGenericType)
            {
                string result = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
                var parameters = t.GetGenericArguments();
                for (int i = 0; i < parameters.Length; i++)
                {
                    result += TypeExtensions.FriendlyName(parameters[i]);
                    if (i < parameters.Length - 1)
                    {
                        result += ", ";
                    }
                }
                result += ">";
                return result;
            }
            else if (t.IsGenericParameter || t.IsGenericTypeDefinition)
            {
                throw new NotSupportedException();
            }
            else
            {
                return t.Name;
            }
        }
    }
}
