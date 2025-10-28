using AutoMapper;
using HTNLShop.Data;
using HTNLShop.ViewModels;

namespace HTNLShop.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterVM,User>()
                .ForMember(kh => kh.FullName, opt => opt.MapFrom(RegisterVM => RegisterVM.FullName))
                .ReverseMap();
        }
    }
}
