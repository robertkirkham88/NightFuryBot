namespace NFB.Service.Vatsim.Mappings
{
    using AutoMapper;

    using NFB.Domain.Bus.Events;
    using NFB.Service.Vatsim.Models;

    /// <summary>
    /// The pilot model mappings.
    /// </summary>
    public class PilotModelMappings : Profile
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotModelMappings"/> class.
        /// </summary>
        public PilotModelMappings()
        {
            this.CreateMap<PilotModel, VatsimPilotUpdatedEvent>()
                .ForMember(p => p.UserId, a => a.UseDestinationValue())
                .ForMember(p => p.Status, a => a.UseDestinationValue());
        }

        #endregion Public Constructors
    }
}