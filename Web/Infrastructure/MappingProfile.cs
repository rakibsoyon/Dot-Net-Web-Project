using AutoMapper;
using Domain;
using Shared.Models;


namespace Web.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();
            //CreateMap<Item, ItemViewModel>().ReverseMap();

            CreateMap<Item, ItemViewModel>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<ItemViewModel, Item>()
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Category, opt => opt.Ignore()); // Ignore mapping of Category
        }
    }
}
