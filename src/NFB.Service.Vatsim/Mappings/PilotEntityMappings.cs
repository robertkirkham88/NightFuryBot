namespace NFB.Service.Vatsim.Mappings
{
    using AutoMapper;

    using NFB.Domain.Bus.Commands;
    using NFB.Service.Vatsim.Entities;

    /// <summary>
    /// The register vatsim command mapping.
    /// </summary>
    public class PilotEntityMapping : Profile
    {
        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PilotEntityMapping"/> class.
        /// </summary>
        public PilotEntityMapping()
        {
            this.CreateMap<RegisterVatsimCommand, PilotEntity>();
        }

        #endregion Public Constructors
    }
}