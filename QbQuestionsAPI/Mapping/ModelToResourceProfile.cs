using AutoMapper;

using QbQuestionsAPI.Domain.Models;
using QbQuestionsAPI.Extensions;
using QbQuestionsAPI.Resources;

namespace QbQuestionsAPI.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<QbQuestion, QbQuestionResource>()
                .ForMember(src => src.Level,
                           opt => opt.MapFrom(src => src.Level.ToDescriptionString()));
        }
    }
}