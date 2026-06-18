using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Exportal_DAL.Utilities
{
    public static class DataTypesUtilities
    {
        public static bool? BoolNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToBoolean(obj);
        }

        public static bool BoolNZ(object obj, bool defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToBoolean(obj);
        }
        public static decimal? DecimalNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToDecimal(obj);
        }
        public static decimal DecimalNZ(object obj, decimal defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToDecimal(obj);
        }

        public static string StringNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return obj.ToString();
        }

        public static string StringNZ(object obj, string defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return obj.ToString();
        }

        public static int? IntNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToInt32(obj);
        }

        public static int IntNZ(object obj, int defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToInt32(obj);
        }
        public static float? FloatNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToSingle(obj);
        }

        public static float FloatNZ(object obj, float defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToSingle(obj);
        }
        public static DateTime? DateTimeNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToDateTime(obj);
        }



        public static DateTime DateTimeNZ(object obj, DateTime defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToDateTime(obj);
        }

        public static byte? ByteNZ(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return Convert.ToByte(obj);
        }

        public static byte ByteNZ(object obj, byte defaultValue)
        {
            if (obj == DBNull.Value)
                return defaultValue;

            return Convert.ToByte(obj);
        }

        /// <summary>
        /// Replace accented characters with unicode character
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ReplaceAccentCharacters(string source)
        {
            string output = string.Empty;

            foreach (char c in source.ToArray())
            {
                switch (c)
                {
                    case 'à':
                    case 'á':
                    case 'â':
                    case 'ã':
                    case 'ä':
                    case 'å':
                        output += "a";
                        break;

                    case 'À':
                    case 'Á':
                    case 'Â':
                    case 'Ã':
                    case 'Ä':
                    case 'Å':
                        output += "A";
                        break;

                    case 'è':
                    case 'é':
                    case 'ê':
                    case 'ë':
                        output += "e";
                        break;

                    case 'È':
                    case 'É':
                    case 'Ê':
                    case 'Ë':
                        output += "E";
                        break;

                    case 'ì':
                    case 'í':
                    case 'î':
                    case 'ï':
                        output += "i";
                        break;

                    case 'Ì':
                    case 'Í':
                    case 'Î':
                    case 'Ï':
                        output += "I";
                        break;

                    case 'ò':
                    case 'ó':
                    case 'ô':
                    case 'õ':
                    case 'ö':
                    case 'ø':
                        output += "o";
                        break;

                    case 'Ò':
                    case 'Ó':
                    case 'Ô':
                    case 'Õ':
                    case 'Ö':
                    case 'Ø':
                        output += "O";
                        break;

                    case 'ù':
                    case 'ú':
                    case 'û':
                    case 'ü':
                        output += "u";
                        break;

                    case 'Ù':
                    case 'Ú':
                    case 'Û':
                    case 'Ü':
                        output += "U";
                        break;

                    case 'ý':
                    case 'ÿ':
                        output += "y";
                        break;

                    case 'Ÿ':
                    case 'Ý':
                        output += "Y";
                        break;

                    case 'ç':
                        output += "c";
                        break;

                    case 'Ç':
                        output += "C";
                        break;

                    case 'ñ':
                        output += "n";
                        break;

                    case 'Ñ':
                        output += "N";
                        break;

                    case 'š':
                        output += "s";
                        break;

                    case 'Š':
                        output += "S";
                        break;

                    case 'ž':
                        output += "z";
                        break;

                    case 'Ž':
                        output += "Z";
                        break;

                    case 'ƒ':
                        output += "f";
                        break;

                    default:
                        output += c;
                        break;
                }
            }

            return output;
        }

        //internal static decimal DecimalNZ(object p1, double p2)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
