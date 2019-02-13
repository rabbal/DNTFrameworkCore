using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.GuardToolkit
{
    public static class Guard
    {
        private const string AgainstMessage = "Assertion evaluation failed with 'false'.";
        private const string ImplementsMessage = "Type '{0}' must implement type '{1}'.";
        private const string InheritsFromMessage = "Type '{0}' must inherit from type '{1}'.";
        private const string IsTypeOfMessage = "Type '{0}' must be of type '{1}'.";
        private const string IsEqualMessage = "Compared objects must be equal.";
        private const string IsPositiveMessage = "Argument '{0}' must be a positive value. Value: '{1}'.";
        private const string IsTrueMessage = "True expected for '{0}' but the condition was False.";
        private const string NotNegativeMessage = "Argument '{0}' cannot be a negative value. Value: '{1}'.";

        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        public static T? NotNull<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        [DebuggerStepThrough]
        public static void ArgumentNotNull(object arg, string argName)
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        [DebuggerStepThrough]
        public static void ArgumentNotEmpty<T>(ICollection<T> arg, string argName)
        {
            if (arg != null && !arg.Any())
                throw new ArgumentException("Collection cannot be null and must have at least one item.", argName);
        }

        [DebuggerStepThrough]
        public static void ArgumentNotEmpty(Guid arg, string argName)
        {
            if (arg == Guid.Empty)
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Argument '{0}' cannot be an empty guid.", argName),
                    argName);
        }

        [DebuggerStepThrough]
        public static void ArgumentInRange<T>(T arg, T min, T max, string argName) where T : struct, IComparable<T>
        {
            if (arg.CompareTo(min) < 0 || arg.CompareTo(max) > 0)
                throw new ArgumentOutOfRangeException(argName,
                    "The argument '{0}' must be between '{1}' and '{2}'.".FormatCurrent(argName, min, max));
        }

        [DebuggerStepThrough]
        public static void ArgumentNotOutOfLength(string arg, int maxLength, string argName)
        {
            if (arg.Trim().Length > maxLength)
                throw new ArgumentException(argName,
                    "Argument '{0}' cannot be more than {1} characters long.".FormatCurrent(argName, maxLength));
        }

        [DebuggerStepThrough]
        public static void ArgumentNotNegative<T>(T arg, string argName, string message = NotNegativeMessage)
            where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) < 0)
                throw new ArgumentOutOfRangeException(argName, message.FormatInvariant(argName, arg));
        }

        [DebuggerStepThrough]
        public static void ArgumentNotZero<T>(T arg, string argName) where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) == 0)
                throw new ArgumentOutOfRangeException(argName,
                    string.Format(CultureInfo.CurrentCulture,
                        "Argument '{0}' must be greater or less than zero. Value: '{1}'.", argName, arg));
        }

        [DebuggerStepThrough]
        public static void InheritsFrom<TBase>(Type type)
        {
            InheritsFrom<TBase>(type, InheritsFromMessage.FormatInvariant(type.FullName, typeof(TBase).FullName));
        }

        [DebuggerStepThrough]
        public static void InheritsFrom<TBase>(Type type, string message)
        {
            if (type.BaseType != typeof(TBase))
                throw new InvalidOperationException(message);
        }

        [DebuggerStepThrough]
        public static void Implements<TInterface>(Type type, string message = ImplementsMessage)
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
                throw new InvalidOperationException(
                    message.FormatInvariant(type.FullName, typeof(TInterface).FullName));
        }

        [DebuggerStepThrough]
        public static void IsTypeOf<TType>(object instance)
        {
            IsTypeOf<TType>(instance, IsTypeOfMessage.FormatInvariant(instance.GetType().Name, typeof(TType).FullName));
        }

        [DebuggerStepThrough]
        public static void IsTypeOf<TType>(object instance, string message)
        {
            if (!(instance is TType))
                throw new InvalidOperationException(message);
        }

        [DebuggerStepThrough]
        public static void IsEqual<TException>(object compare, object instance, string message = IsEqualMessage)
            where TException : Exception
        {
            if (!compare.Equals(instance))
                throw (TException) Activator.CreateInstance(typeof(TException), message);
        }

        [DebuggerStepThrough]
        public static void ArgumentIsPositive<T>(T arg, string argName, string message = IsPositiveMessage)
            where T : struct, IComparable<T>
        {
            if (arg.CompareTo(default(T)) < 1)
                throw new ArgumentOutOfRangeException(argName, message.FormatInvariant(argName));
        }

        [DebuggerStepThrough]
        public static void ArgumentIsTrue(bool arg, string argName, string message = IsTrueMessage)
        {
            if (!arg)
                throw new ArgumentException(message.FormatInvariant(argName), argName);
        }


        [DebuggerStepThrough]
        public static void ArgumentIsEnumType(Type type, string argName)
        {
            ArgumentNotNull(type, argName);
            if (!type.IsEnum)
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentCulture, "Type '{0}' must be a valid Enum type.", type.FullName),
                    argName);
        }

        [DebuggerStepThrough]
        public static void ArgumentIsEnumType<TEnum>(object arg, string argName) where TEnum : struct
        {
            ArgumentNotNull(arg, argName);
            if (!Enum.IsDefined(typeof(TEnum), arg))
                throw new ArgumentOutOfRangeException(argName,
                    string.Format(CultureInfo.CurrentCulture,
                        "The value of the argument '{0}' provided for the enumeration '{1}' is invalid.", argName,
                        typeof(TEnum).FullName));
        }

        [DebuggerStepThrough]
        public static void PagingArgsValid(int indexArg, long sizeArg, string indexArgName, string sizeArgName)
        {
            ArgumentNotNegative(indexArg, indexArgName, "PageIndex cannot be below 0");
            if (indexArg > 0)
                ArgumentIsPositive(sizeArg, sizeArgName,
                    "PageSize cannot be below 1 if a PageIndex greater 0 was provided.");
            else
                ArgumentNotNegative(sizeArg, sizeArgName);
        }

        [DebuggerStepThrough]
        public static T ArgumentNotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        [DebuggerStepThrough]
        public static T? ArgumentNotNull<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(parameterName);
            return value;
        }

        [DebuggerStepThrough]
        public static string ArgumentNotEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(parameterName);
            return value;
        }

        [DebuggerStepThrough]
        public static bool HasConsecutiveChars(string inputText, int sequenceLength = 3)
        {
            var charEnumerator = StringInfo.GetTextElementEnumerator(inputText);
            var currentElement = string.Empty;
            var count = 1;
            while (charEnumerator.MoveNext())
                if (currentElement == charEnumerator.GetTextElement())
                {
                    if (++count >= sequenceLength)
                        return true;
                }
                else
                {
                    count = 1;
                    currentElement = charEnumerator.GetTextElement();
                }

            return false;
        }

    }
}