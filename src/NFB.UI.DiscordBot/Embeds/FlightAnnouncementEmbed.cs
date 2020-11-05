namespace NFB.UI.DiscordBot.Embeds
{
    using System;

    using Discord;

    using NFB.Domain.Bus.DTOs;

    /// <summary>
    /// The flight announcement embed.
    /// </summary>
    public static class FlightAnnouncementEmbed
    {
        #region Public Methods

        /// <summary>
        /// Create an embed.
        /// </summary>
        /// <param name="origin">
        /// The origin.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <returns>
        /// The <see cref="Embed"/>.
        /// </returns>
        public static Embed CreateEmbed(AirportEntityDto origin, AirportEntityDto destination, DateTime startTime)
        {
            var embedBuilder = new EmbedBuilder { Color = Color.Red, Title = $"{origin.Name} to {destination.Name}" };

            embedBuilder.AddField("Status", "Not started");
            embedBuilder.AddField("Origin", origin.ICAO, true);
            embedBuilder.AddField("Destination", destination.ICAO, true);
            embedBuilder.AddField("Planned departure time", $"{startTime:ddd dd MMM yyyy HH:mm} UTC");
            embedBuilder.AddField("Map", "TBC");

            return embedBuilder.Build();
        }

        #endregion Public Methods
    }
}