using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace br.com.Bonus630DevToolsBar.RunCommandDocker
{
    public static class Utils
    {
        public static bool IsCollectionOfValueTypeOrString(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            Type objType = obj.GetType();

            // Verificar se é uma coleção genérica (List<T> ou T[])
            if (objType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(objType))
            {
                Type itemType = objType.GetGenericArguments()[0];
                return IsValueTypeOrString(itemType);
            }

            // Verificar se é uma array
            if (objType.IsArray)
            {
                Type elementType = objType.GetElementType();
                return IsValueTypeOrString(elementType);
            }

            // Verificar se é um ArrayList
            if (typeof(ArrayList).IsAssignableFrom(objType))
            {
                ArrayList arrayList = (ArrayList)obj;
                if (arrayList.Count > 0)
                {
                    Type elementType = arrayList[0].GetType();
                    return IsValueTypeOrString(elementType);
                }
            }

            return false;
        }

        public static bool IsValueTypeOrString(Type type)
        {
            return type.IsValueType || type == typeof(string);
        }

        public static string ConcatenateValues(object collection)
        {
            Type collectionType = collection.GetType();

            // Verifica se é uma coleção genérica de int
            if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type itemType = collectionType.GetGenericArguments()[0];

                if (itemType == typeof(int))
                {
                    // Concatena inteiros
                    List<int> intCollection = (List<int>)collection;
                    return string.Join(",", intCollection);
                }
                else if (itemType == typeof(string))
                {
                    // Concatena strings
                    List<string> stringCollection = (List<string>)collection;
                    return string.Join(",", stringCollection);
                }
            }

            // Verifica se é uma array de int ou string
            if (collectionType.IsArray)
            {
                Type elementType = collectionType.GetElementType();

                if (elementType == typeof(int))
                {
                    int[] intArray = (int[])collection;
                    return string.Join(",", intArray);
                }
                else if (elementType == typeof(string))
                {
                    string[] stringArray = (string[])collection;
                    return string.Join(",", stringArray);
                }
            }

            // Verifica se é um ArrayList
            if (typeof(ArrayList).IsAssignableFrom(collectionType))
            {
                ArrayList arrayList = (ArrayList)collection;
                return string.Join(",", arrayList.ToArray());
            }

            return string.Empty;
        }

        public static bool IsTypeAny(object obj,params Type[] types)
        {
            Type t = obj.GetType();
            for (int i = 0; i <= types.Length; i++)
            {
                if (t.Equals(types[i]))
                    return true;
            }
            return false;
        }

    }
}
