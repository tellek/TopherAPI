using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Utilities
{
    public static class ObjectParsing
    {
        public static void AllPropsToLogContext(object theObject)
        {
            if (theObject == null) return;

            foreach (PropertyInfo pi in theObject.GetType().GetProperties())
            {
                LogContext.PushProperty($"-{pi.Name}", pi.GetValue(theObject));
            }
        }

        public static Dictionary<string, string> AllPropsToDictionary(object theObject)
        {
            if (theObject == null) return null;

            var result = new Dictionary<string, string>();

            foreach (PropertyInfo pi in theObject.GetType().GetProperties())
            {
                result.Add(pi.Name, pi.GetValue(theObject).ToString());
            }

            return result;
        }
    }
}
