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
                                 usersInChannel.Select(p => $"- {p.Username}"));

            embedBuilder.AddField("Status", "Started");
            embedBuilder.AddField("Origin", origin.ICAO, true);
            embedBuilder.AddField("Destination", destination.ICAO, true);
            embedBuilder.AddField("Planned departure time", $"{startTime:g} UTC");
            embedBuilder.AddField("Pilots", pilotsText);
            embedBuilder.AddField("Map", "TBC");

            if (vatsimData.Any())
            {
                foreach (var item in vatsimData)
                {
                    embedBuilder.AddField("vatsim item", $"{item.Longitude} - {item.Latitude}");
                }
            }

            return embedBuilder.Build();
        }

        #endregion Public Methods
    }
}