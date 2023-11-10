using System.Reflection;
using WebApi.IntegrationTests.Attributes;

namespace WebApi.IntegrationTests.Extentions
{
    public static class ErrorCodeExts
    {
		/// <summary>
		/// Gets the <see cref="DescriptionAttribute" /> of an <see cref="Enum" /> 
		/// type value.
		/// </summary>
		/// <param name="value">The <see cref="Enum" /> type value.</param>
		/// <returns>A string containing the text of the
		/// <see cref="DescriptionAttribute"/>.</returns>
		public static string GetCode(this Enum value)
        {
            var attribute = GetAttribute(value);
            return attribute?.Code ?? value.ToString();
        }

        /// <summary>
        /// Gets the <see cref="DescriptionAttribute" /> of an <see cref="Enum" /> 
        /// type value.
        /// </summary>
        /// <param name="value">The <see cref="Enum" /> type value.</param>
        /// <returns>A string containing the text of the
        /// <see cref="DescriptionAttribute"/>.</returns>
        public static string GetMessage(this Enum value)
        {
            var attribute = GetAttribute(value);
            return attribute?.Message ?? value.ToString();
        }

        public static ErrorCodeAttribute GetAttribute(this Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);

            if (fieldInfo == null)
            {
                throw new ArgumentNullException($"FieldInfo. Value = {value} probably is not enum value");
            }

            ErrorCodeAttribute[] attributes =
                (ErrorCodeAttribute[])
                fieldInfo.GetCustomAttributes(typeof(ErrorCodeAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0];
            }
            return null;
        }
    }
}
