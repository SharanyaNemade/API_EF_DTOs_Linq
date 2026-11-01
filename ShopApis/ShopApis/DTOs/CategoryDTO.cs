using System.ComponentModel.DataAnnotations;

public class CategoryCreateDto
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }
}

public class CategoryUpdateDto : CategoryCreateDto { }

// It means: “CategoryUpdateDto is a new class that copies everything from CategoryCreateDto.” This is called inheritance.

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ProductCount { get; set; }
}