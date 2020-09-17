namespace NFB.UI.DiscordBot.Embeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;

    using NFB.Domain.Bus.DTOs;
    using NFB.UI.DiscordBot.Models;

    /// <summary>
    /// The flight starting embed.
    /// </summary>
    public class FlightActiveEmbed
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
        /// <param name="voiceChannel">
        /// The voice channel.
        /// </param>
        /// <param name="vatsimData">
        /// The vatsim Data.
        /// </param>
        /// <returns>
        /// The <see cref="Embed"/>.
        /// </returns>
        public static async Task<Embed> CreateEmbed(AirportEntityDto origin, AirportEntityDto destination, DateTime startTime, IGuildChannel voiceChannel, IList<VatsimPilotData> vatsimData)
        {
            var embedBuilder = new EmbedBuilder { Color = Color.Green, Title = $"{origin.Name} to {destination.Name}" };
            var usersInChannel = await voiceChannel.GetUsersAsync().FlattenAsync();
            var pilotsText = $"Join {voiceChannel.Name}\r\n" + string.Join(
                                 "\r\n",
                                 usersInChannel.Select(p => $"- {p.Nickname ?? p.Username}"));

            embedBuilder.AddField("Status", "Started");
            embedBuilder.AddField("Origin", origin.ICAO, true);
            embedBuilder.AddField("Destination", destination.ICAO, true);
            embedBuilder.AddField("Planned departure time", $"{startTime:g} UTC");
            embedBuilder.AddField("Pilots", pilotsText);

            if (!vatsimData.Any()) return embedBuilder.Build();

            var pilotMapData = string.Join("%7C%7C", vatsimData.Select(p => $"{p.Latitude},{p.Longitude}%7Cmarker-{ColorToHex(new Color(p.AssignedColor))}-A"));
            var url = $"https://open.mapquestapi.com/staticmap/v5/map?locations={origin.Latitude},{origin.Longitude}%7Cmarker-start%7C%7C{destination.Latitude},{destination.Longitude}%7Cmarker-end%7C%7C{pilotMapData}&key=z8WHXvhF50CDEdE3GOrleDlWtwOOZVjG";
            embedBuilder.WithImageUrl(url);

            return embedBuilder.Build();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// The color to hex.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string ColorToHex(Color color)
        {
            return color.R.ToString("X2") + color.B.ToString("X2") + color.G.ToString("X2");
        }

        #endregion Private Methods
    }
}