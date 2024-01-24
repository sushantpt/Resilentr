namespace Resilentr.src.Common
{
    /// <summary>
    /// Types of attributes
    /// </summary>
    public enum AttributeProperty
    {
        /// <summary>
        /// A stealthy attribute that silences any exception. Http Status 200 Ok.
        /// </summary>
        Silent = 1,
        /// <summary>
        /// Loud error if any kinda exception is thrown. 
        /// </summary>
        Loud = 2,
        /// <summary>
        /// Default. Does nothing. 
        /// </summary>
        Default = 3,
        /// <summary>
        /// Call to this method will throw an exception doesn't matter it executes or fails. Http status 500 InternalServerError.
        /// </summary>
        Faulted = 4
    }
}
