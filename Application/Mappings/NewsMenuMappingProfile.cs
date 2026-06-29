using AutoMapper;
using Application.Request.Menu;
using Application.Request.News;
using Domain.Entities;

namespace Application.Mappings
{
    public class NewsMenuMappingProfile : Profile
    {
        public NewsMenuMappingProfile()
        {
            CreateMap<UpdateNewsRequest, News>().ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<NewsUpdateItem, News>().ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateMenuRequest, Menu>().ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<MenuUpdateItem, Menu>().ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}