using AutoMapper;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.Mapping
{
    public class ResourceToModelProfile : Profile
    {
        public ResourceToModelProfile()
        {
            CreateMap<SaveQbQuestionsResource, QbQuestion>()
                .ForMember(src => src.Level,
                           opt => opt.MapFrom(src => (ETournamentLevel)src.Level));
        }
    }
}