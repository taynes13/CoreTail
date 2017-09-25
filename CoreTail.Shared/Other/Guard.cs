using System;

namespace CoreTail.Shared.Other
{
    internal class Guard
    {
        public static T ArgumentNotNull<T>(T argumentValue, string argumentName = null) where T : class
        {
            return argumentValue ?? throw new ArgumentNullException(argumentName ?? nameof(argumentValue));
        }
    }
}
