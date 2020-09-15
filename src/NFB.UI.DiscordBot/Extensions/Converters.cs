namespace NFB.UI.DiscordBot.Extensions
{
    using System;

    /// <summary>
    /// The converters.
    /// </summary>
    public static class Converters
    {
        #region Public Methods

        /// <summary>
        /// Convert to Guid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        public static Guid ToGuid(this ulong value)
        {
            byte[] guidData = new byte[16];
            Array.Copy(BitConverter.GetBytes(value), guidData, 8);
            return new Guid(guidData);
        }

        /// <summary>
        /// Convert to ulong.
        /// </summary>
        /// <param name="guid">
        /// The guid.
        /// </param>
        /// <returns>
        /// The <see cref="ulong"/>.
        /// </returns>
        public static ulong ToULong(this Guid guid)
        {
            if (BitConverter.ToUInt64(guid.ToByteArray(), 8) != 0)
                throw new OverflowException("Value was either too large or too small for an Int64.");
            return BitConverter.ToUInt64(guid.ToByteArray(), 0);
        }

        #endregion Public Methods
    }
}