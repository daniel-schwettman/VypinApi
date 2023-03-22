namespace VypinApi.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Asset")]
    public partial class Asset
    {
        public int AssetId { get; set; }

        public string Name { get; set; }

        public int? SlotId { get; set; }

        public int TagId { get; set; }

        public bool IsActive { get; set; }

        public string AssetIdentifier { get; set; }
    }
}
