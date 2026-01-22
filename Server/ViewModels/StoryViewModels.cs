using System.ComponentModel.DataAnnotations;
using Server.Models;

namespace Server.ViewModels;

public class CreateStoryViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên bé")]
    [StringLength(200, ErrorMessage = "Tên không quá 200 ký tự")]
    [Display(Name = "Tên bé")]
    public string ChildName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tuổi")]
    [Range(1, 15, ErrorMessage = "Tuổi phải từ 1 đến 15")]
    [Display(Name = "Tuổi")]
    public int ChildAge { get; set; } = 5;

    [Display(Name = "Chủ đề")]
    [StringLength(200)]
    public string? Theme { get; set; }

    [Display(Name = "Nội dung / Ý tưởng")]
    public string? Content { get; set; }

    [Display(Name = "Audio (Base64)")]
    public string? AudioBase64 { get; set; }

    [Display(Name = "Loại audio")]
    public string? AudioMimeType { get; set; }

    public bool UseAudio { get; set; } = false;
}

public class StoryListViewModel
{
    public List<Story> Stories { get; set; } = new();
    public string? SearchTerm { get; set; }
    public string? FilterTheme { get; set; }
    public string? FilterChildName { get; set; }
    public bool? FilterFavorite { get; set; }
    public List<string> AvailableThemes { get; set; } = new();
    public List<string> AvailableChildNames { get; set; } = new();
}

public class StoryDetailsViewModel
{
    public Story Story { get; set; } = null!;
    public int CurrentPage { get; set; } = 1;
    public bool IsOwner { get; set; }
    public string? ShareUrl { get; set; }
}

public class EditStoryViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    [StringLength(500)]
    [Display(Name = "Tiêu đề")]
    public string Title { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [Display(Name = "Chủ đề")]
    public string? Theme { get; set; }
}

public class EditPageViewModel
{
    public Guid StoryId { get; set; }
    public int PageNumber { get; set; }
    
    [Required]
    [Display(Name = "Nội dung trang")]
    public string Content { get; set; } = string.Empty;
    
    public string? ImageUrl { get; set; }
}
