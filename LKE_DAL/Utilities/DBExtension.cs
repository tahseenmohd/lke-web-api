using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LKE_DAL.Utilities
{
    public static class DBExtension
    {
        public static object ChangeTypeTo(this object value, Type conversionType)
        {
            if (conversionType == (Type)null)
                throw new ArgumentNullException("conversionType");
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return (object)null;
                conversionType = new NullableConverter(conversionType).UnderlyingType;
            }
            else
            {
                if (conversionType == typeof(Guid))
                    return (object)new Guid(value.ToString());
                if (conversionType == typeof(long) && value.GetType() == typeof(int))
                    throw new InvalidOperationException("Can't convert an Int64 (long) to Int32(int). If you're using SQLite - this is probably due to your PK being an INTEGER, which is 64bit. You'll need to set your key to long.");
            }
            return Convert.ChangeType(value, conversionType);
        }

        public static void Load<T>(this IDataReader rdr, T item)
        {
            Type type1 = typeof(T);
            PropertyInfo[] properties = type1.GetProperties();
            FieldInfo[] fields = type1.GetFields();
            FieldInfo fieldInfo = (FieldInfo)null;
            for (int i = 0; i < rdr.FieldCount; ++i)
            {
                string pName = rdr.GetName(i);
                if(pName=="Inactive")
                {
                    fieldInfo = Enumerable.SingleOrDefault<FieldInfo>((IEnumerable<FieldInfo>)fields, (Func<FieldInfo, bool>)(x => x.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase)));
                    fieldInfo.SetValue((object)item, DBExtension.ChangeTypeTo(rdr.GetValue(i),true.GetType() ));
                }
                PropertyInfo propertyInfo = Enumerable.SingleOrDefault<PropertyInfo>((IEnumerable<PropertyInfo>)properties, (Func<PropertyInfo, bool>)(x => x.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase)));
                if (propertyInfo == (PropertyInfo)null)
                    fieldInfo = Enumerable.SingleOrDefault<FieldInfo>((IEnumerable<FieldInfo>)fields, (Func<FieldInfo, bool>)(x => x.Name.Equals(pName, StringComparison.InvariantCultureIgnoreCase)));
                if (propertyInfo != (PropertyInfo)null && !DBNull.Value.Equals(rdr.GetValue(i)))
                {
                    Type type2 = rdr.GetValue(i).GetType();
                    if (type2 == typeof(bool))
                        propertyInfo.SetValue((object)item, (object)(int)(rdr.GetValue(i).ToString() == "1" ? 1 : 0), (object[])null);
                    else if (propertyInfo.PropertyType == typeof(Guid))
                        propertyInfo.SetValue((object)item, (object)rdr.GetGuid(i), (object[])null);
                    else
                        propertyInfo.SetValue((object)item, DBExtension.ChangeTypeTo(rdr.GetValue(i), type2), (object[])null);
                }
                else if (fieldInfo != (FieldInfo)null && !DBNull.Value.Equals(rdr.GetValue(i)))
                {
                    Type type2 = rdr.GetValue(i).GetType();
                    if (type2 == typeof(bool))
                        fieldInfo.SetValue((object)item, (object)(int)(rdr.GetValue(i).ToString() == "1" ? 1 : 0));
                    else if (fieldInfo.FieldType == typeof(Guid))
                        fieldInfo.SetValue((object)item, (object)rdr.GetGuid(i));
                    else
                        fieldInfo.SetValue((object)item, DBExtension.ChangeTypeTo(rdr.GetValue(i), type2));
                }
            }
        }

        public static T SingleOrDefault<T>(this IDataReader rdr) where T : new()
        {
            T obj = default(T);
            List<T> list = DBExtension.ToList<T>(rdr);
            if (list.Count > 0)
                obj = list[0];
            return obj;
        }

        public static List<T> ToList<T>(this IDataReader rdr) where T : new()
        {
            List<T> list = new List<T>();
            Type type = typeof(T);
            while (rdr.Read())
            {
                T obj1;
                if ((object)default(T) != null)
                {
                    T obj2 = default(T);
                    T obj3;
                    obj1 = obj3 = obj2;
                }
                else
                    obj1 = Activator.CreateInstance<T>();
                T obj4 = obj1;
                DBExtension.Load<T>(rdr, obj4);
                list.Add(obj4);
            }
            return list;
        }
    }
}
