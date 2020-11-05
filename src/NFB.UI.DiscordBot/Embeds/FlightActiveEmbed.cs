namespace NFB.UI.DiscordBot.Embeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Discord;

    using GeoCoordinatePortable;

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
        /// <param name="existingImage">
        /// The existing Image.
        /// </param>
        /// <returns>
        /// The <see cref="Embed"/>.
        /// </returns>
        public static async Task<Embed> CreateEmbed(AirportEntityDto origin, AirportEntityDto destination, DateTime startTime, IGuildChannel voiceChannel, IList<VatsimPilotModel> vatsimData, EmbedImage? existingImage = null)
        {
            var embedBuilder = new EmbedBuilder { Color = Color.Green, Title = $"{origin.Name} to {destination.Name}" };
            var usersInChannel = await voiceChannel.GetUsersAsync().FlattenAsync();
            var pilotsText = $"Join voice channel: {voiceChannel.Name}\r\n" + string.Join(
                                 "\r\n",
                                 usersInChannel.Select(p =>
                                     {
                                         var pilot = vatsimData.FirstOrDefault(m => m.UserId == p.Id);

                                         if (pilot == null || pilot.Status == "Offline")
                                         {
                                             return $"- {p.Nickname ?? p.Username}";
                                         }

                                         var remainingDistance = GetRemainingDistance(destination, vatsimData, p.Id);
                                         var etaTime = GetRemainingTime(remainingDistance, pilot.FlightSpeed);
                                         var eta = pilot.FlightSpeed < 100 ? "N/A" : $"{etaTime:HHmm}";

                                         return $"- {p.Nickname ?? p.Username} - {pilot.CallSign} (Dist: {remainingDistance}nm, Hdg: {pilot.FlightHeading}, Spd: {pilot.FlightSpeed}, Alt: {pilot.FlightAltitude}, ETA: {eta})";
                                     }));

            embedBuilder.AddField("Status", "Started");
            embedBuilder.AddField("Origin", origin.ICAO, true);
            embedBuilder.AddField("Destination", destination.ICAO, true);
            embedBuilder.AddField("Planned departure time", $"{startTime:ddd dd MMM yyyy HH:mm} UTC");
            embedBuilder.AddField("Pilots", pilotsText);

            if (!vatsimData.Any()) return embedBuilder.Build();
            try
            {
                var voiceChannelUsers = await voiceChannel.GetUsersAsync().FlattenAsync();
                var pilotMapData = string.Join(
                    "%7C%7C",
                    vatsimData.Where(p => p.Status == "Online").Select(
                        p =>
                            $"{p.Latitude},{p.Longitude}%7Cmarker-{ColorToHex(new Color(p.AssignedColor))}-{GetPilotMarker(voiceChannelUsers, p.UserId)}"));
                var url =
                    $"https://open.mapquestapi.com/staticmap/v5/map?locations={origin.Latitude},{origin.Longitude}%7Cmarker-start%7C%7C{destination.Latitude},{destination.Longitude}%7Cmarker-end%7C%7C{pilotMapData}&size=400,300&key=z8WHXvhF50CDEdE3GOrleDlWtwOOZVjG";
                embedBuilder.WithImageUrl(url);
            }
            catch
            {
                // ignored
            }

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

        /// <summary>
        /// The get pilot marker.
        /// </summary>
        /// <param name="voiceChannelUsers">
        /// The voice channel users.
        /// </param>
        /// <param name="id">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPilotMarker(IEnumerable<IGuildUser> voiceChannelUsers, ulong id)
        {
            var user = voiceChannelUsers.FirstOrDefault(p => p.Id == id);

            return user == null ? string.Empty : GetPilotMarker(user.Nickname ?? user.Username);
        }

        /// <summary>
        /// The get pilot marker.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetPilotMarker(string name)
        {
            var rtn = name.TakeWhile(char.IsLetter).ToArray().First();

            return new string(rtn.ToString());
        }

        /// <summary>
        /// Get the remaining distance.
        /// </summary>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="pilots">
        /// The pilots.
        /// </param>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        private static double GetRemainingDistance(AirportEntityDto destination, IList<VatsimPilotModel> pilots, ulong user)
        {
            var pilot = pilots.FirstOrDefault(p => p.UserId == user);

            if (pilot == null)
                return double.NaN;

            var destinationCoordinates = new GeoCoordinate(destination.Latitude, destination.Longitude);
            var pilotCoordinates = new GeoCoordinate(pilot.Latitude, pilot.Longitude);

            return Math.Round(pilotCoordinates.GetDistanceTo(destinationCoordinates) * 0.000539957, 0); // meters to miles.
        }

        /// <summary>
        /// Get the ETA.
        /// </summary>
        /// <param name="nm">
        /// The nm.
        /// </param>
        /// <param name="knots">
        /// The knots.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        private static DateTime GetRemainingTime(double nm, int knots)
        {
            var milesRemaining = nm * 1.15078;
            var milesPerHour = knots * 1.15078;

            var minutesRemaining = (milesRemaining * 60) / milesPerHour;

            if (double.IsInfinity(minutesRemaining))
                return DateTime.UtcNow;

            return DateTime.UtcNow.AddMinutes(minutesRemaining);
        }

        #endregion Private Methods
    }
}