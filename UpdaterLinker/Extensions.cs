// This file is part of Mystery Dungeon eXtended.

// Mystery Dungeon eXtended is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Mystery Dungeon eXtended is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with Mystery Dungeon eXtended.  If not, see <http://www.gnu.org/licenses/>.

using System;

namespace PMDCP.Updater.Linker
{
    public static class Extensions
    {
        public static int ToInt(this string str)
        {
            return str != null && int.TryParse(str, out int result) == true ? result : 0;
        }

        public static int ToInt(this string str, int defaultVal)
        {
            return str != null && int.TryParse(str, out int result) == true ? result : defaultVal;
        }

        public static long ToLong(this string str)
        {
            return str != null && long.TryParse(str, out long result) == true ? result : 0;
        }

        public static long ToLong(this string str, long defaultVal)
        {
            return str != null && long.TryParse(str, out long result) == true ? result : defaultVal;
        }

        public static double ToDbl(this string str)
        {
            return str != null && double.TryParse(str, out double result) == true ? result : 0;
        }

        public static double ToDbl(this string str, double defaultVal)
        {
            return str != null && double.TryParse(str, out double result) == true ? result : defaultVal;
        }

        public static string ToIntString(this bool boolval)
        {
            if (boolval == true)
                return "1";
            else
                return "0";
        }

        public static bool IsNumeric(this string str)
        {
            return int.TryParse(str, out int result);
        }

        public static ulong ToUlng(this string str)
        {
            return ulong.TryParse(str, out ulong result) == true ? result : 0;
        }

        public static bool ToBool(this string str)
        {
            switch (str.ToLower())
            {
                case "true":
                    return true;

                case "false":
                    return false;

                case "1":
                    return true;

                case "0":
                    return false;

                default:
                    return false;
            }
        }

        public static DateTime? ToDate(this string date)
        {
            return DateTime.TryParse(date, out DateTime tmpDate) ? (DateTime?)tmpDate : null;
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
    }
}