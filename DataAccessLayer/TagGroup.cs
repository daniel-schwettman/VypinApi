namespace VypinApi.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("TagGroup")]
    public partial class TagGroup
    {
        public int TagGroupId { get; set; }
        public int TagId { get; set; }
        public int GroupId { get; set; }
    }
}
