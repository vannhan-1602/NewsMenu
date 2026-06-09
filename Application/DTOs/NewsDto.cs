using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record NewsDto(
        Guid Id,
        string Title,
        string Content,
        string? Summary,
        bool IsPublished,
        DateTime CreatedAt,
        List<MenuDto> Menus
    );

    public record MenuDto(
        Guid Id,
        string Name,
        string Slug,
        int DisplayOrder
    );
}
