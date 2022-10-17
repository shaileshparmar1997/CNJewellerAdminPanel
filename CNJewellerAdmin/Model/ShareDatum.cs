using System;
using System.Collections.Generic;

namespace CNJewellerAdmin.Model
{
    public partial class ShareDatum
    {
        public int Id { get; set; }
        public Guid? SharedGuid { get; set; }
        public string SharedData { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? ThumbnailLink { get; set; }
        public string Mimetype { get; set; } = null!;
    }
}
