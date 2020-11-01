namespace NFB.UI.DiscordBot.Mappings
{
    using System.Collections.Generic;

    using AutoMapper;

    using Discord;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.States;

    /// <summary>
    /// The flight state mapping.
    /// </summary>
    public class FlightStateMapping : Profile
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FlightStateMapping"/> class.
        /// </summary>
        public FlightStateMapping()
        {
            this.CreateMap<FlightCreatedEvent, FlightState>()
                .AfterMap(
                    (s, d) =>
                        {
                            d.AvailableColors = new List<uint>
                                                    {
                                                        Color.Blue.RawValue,
                                                        Color.Gold.RawValue,
                                                        Color.Magenta.RawValue,
                                                        Color.Teal.RawValue,
                                                        Color.Orange.RawValue,
                                                        Color.Purple.RawValue,
                                                        Color.LightOrange.RawValue,
                                                        Color.LightGrey.RawValue
                                                    };
                        });
        }

        #endregion Public Constructors
    }
}