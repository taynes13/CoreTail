using System;
using System.Collections.Generic;
using System.Text;

namespace CoreTail.Shared
{
    internal class Guard
    {
        public static T ArgumentNotNull<T>(T argumentValue, string argumentName = null) where T: class
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName ?? nameof(argumentValue));
            }

            return argumentValue;
        }
    }
}
