﻿
namespace Cukcuk.Core.Entities
{
    public class DocumentBlock
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public int Level { get; set; }
        public int ContentType { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

    }
}
