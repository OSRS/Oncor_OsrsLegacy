using System;
using System.Collections.Generic;
using System.Text;

namespace Osrs.Data
{
    public static class SqlLanguageUtils
    {
        public static string FormatInList(IEnumerable<Guid> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (Guid id in vals)
            {
                sb.Append("'");
                sb.Append(id.ToString());
                sb.Append("',");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<string> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (string id in vals)
            {
                sb.Append("'");
                sb.Append(id.ToString());
                sb.Append("',");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<int> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (int id in vals)
            {
                sb.Append(id.ToString());
                sb.Append(",");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<long> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (long id in vals)
            {
                sb.Append(id.ToString());
                sb.Append(",");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<float> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (float id in vals)
            {
                sb.Append(id.ToString());
                sb.Append(",");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<double> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (double id in vals)
            {
                sb.Append(id.ToString());
                sb.Append(",");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }
        public static string FormatInList(IEnumerable<DateTime> vals)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("IN (");
            foreach (DateTime id in vals)
            {
                sb.Append("'");
                sb.Append(id.ToString());
                sb.Append("',");
            }
            sb.Length = sb.Length - 1; //remove trailing comma
            sb.Append(")");
            return sb.ToString();
        }

        public static string FormatBetween(int start, int end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BETWEEN ");
            sb.Append(start);
            sb.Append(" AND ");
            sb.Append(end);
            return sb.ToString();
        }
        public static string FormatBetween(long start, long end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BETWEEN ");
            sb.Append(start);
            sb.Append(" AND ");
            sb.Append(end);
            return sb.ToString();
        }
        public static string FormatBetween(float start, float end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BETWEEN ");
            sb.Append(start);
            sb.Append(" AND ");
            sb.Append(end);
            return sb.ToString();
        }
        public static string FormatBetween(double start, double end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BETWEEN ");
            sb.Append(start);
            sb.Append(" AND ");
            sb.Append(end);
            return sb.ToString();
        }
        public static string FormatBetween(DateTime start, DateTime end)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("BETWEEN ");
            sb.Append(start);
            sb.Append(" AND ");
            sb.Append(end);
            return sb.ToString();
        }

        public static string FormatGroupBy(string name)
        {
            return "GROUP BY " + name;
        }
        public static string FormatGroupBy(IEnumerable<string> names)
        {
            if (names == null)
                return null;

            StringBuilder sb = new StringBuilder();
            sb.Append("GROUP BY ");
            foreach (string s in names)
            {
                sb.Append(s);
                sb.Append(',');
            }
            sb.Length = sb.Length - 1;
            return sb.ToString();
        }
    }
}
