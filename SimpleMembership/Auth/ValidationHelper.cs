using System;
using System.Globalization;

namespace SimpleMembership.Auth.OAuth2
{
    public static class ValidationHelper
    {
        /// <summary>
        /// Verifies something about the argument supplied to a method.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to true to avoid an exception.</param>
        /// <param name="message">The message to use in the exception if the condition is false.</param>
        /// <param name="args">The string formatting arguments, if any.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        public static void VerifyArgument(bool condition, string message, params object[] args)
        {
            //    Requires.NotNull(args, "args");
            // Assumes.True(message != null);
            if (!condition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args));
            }
        }
    }
}