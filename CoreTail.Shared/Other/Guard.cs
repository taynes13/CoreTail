using System;

namespace CoreTail.Shared.Other
{
    internal class Guard
    {
        public static T ArgumentNotNull<T>(T argumentValue, string argumentName = null) where T : class
        {
            if (argumentValue == null)
                throw new ArgumentNullException(argumentName ?? nameof(argumentValue));

            return argumentValue;
        }
    }
}
