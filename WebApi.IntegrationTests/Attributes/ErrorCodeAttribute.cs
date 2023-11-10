namespace WebApi.IntegrationTests.Attributes
{
    /// <summary>
	/// Provides a description for an enumerated type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    public class ErrorCodeAttribute : Attribute
    {
        /// <summary>Gets the description stored in this attribute.</summary>
        /// <value>The description stored in the attribute.</value>
        public virtual string Code { get; }

        /// <summary>Gets the message stored in this attribute.</summary>
        /// <value>The message stored in the attribute.</value>
        public virtual string Message { get; }

        /// <summary>Initializes a new instance of the <see cref="ErrorCodeAttribute"/> class.</summary>
        /// <param name="code">The description to store in this attribute.</param>
        /// <param name="message">The message to store in this attribute.</param>
        public ErrorCodeAttribute(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
