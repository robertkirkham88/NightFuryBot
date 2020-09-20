namespace NFB.UI.DiscordBot.Mappings
{
    using AutoMapper;

    using NFB.Domain.Bus.Events;
    using NFB.UI.DiscordBot.Models;

    /// <summary>
    /// The vatsim pilot updated mapping.
    /// </summary>
    public class VatsimPilotUpdatedMapping : Profile
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VatsimPilotUpdatedMapping"/> class.
        /// </summary>
        public VatsimPilotUpdatedMapping()
        {
            this.CreateMap<VatsimPilotUpdatedEvent, VatsimPilotModel>()
                .ForMember(p => p.AssignedColor, a => a.UseDestinationValue());
        }

        #endregion Public Constructors
    }
}