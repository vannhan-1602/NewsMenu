using Application.Mappings;
using Application.Request.Menu;
using Application.Request.News;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class NewsMenuMappingProfile : Profile
    {
        public NewsMenuMappingProfile()
        {
            // Create: ghi tất cả field
            CreateMap<UpdateNewsRequest, News>().ConvertUsing(new NullValueIgnoringConverter<UpdateNewsRequest, News>());
            CreateMap<NewsUpdateItem, News>().ConvertUsing(new NullValueIgnoringConverter<NewsUpdateItem, News>());
            CreateMap<UpdateMenuRequest, Menu>().ConvertUsing(new NullValueIgnoringConverter<UpdateMenuRequest, Menu>());
            CreateMap<MenuUpdateItem, Menu>().ConvertUsing(new NullValueIgnoringConverter<MenuUpdateItem, Menu>());
        }
    }
}