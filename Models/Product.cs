using System;
using System.Collections.Generic;
using System.IO;

namespace prakt15_TRPO.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public int Stock { get; set; }

    public decimal Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public bool IsLowStock => Stock < 10;

    public string? ImagePath { get; set; }

    public string FullImagePath
    {
        get
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(ImagePath))
                return Path.Combine(baseDir, "Images", "default.png");

            string fullPath = Path.Combine(baseDir, ImagePath);

            return File.Exists(fullPath) ? fullPath : Path.Combine(baseDir, "Images", "default.png");
        }
    }
}
