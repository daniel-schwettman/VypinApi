namespace VypinApi.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountGroupTag")]
    public partial class AccountGroupTag
    {
        public int AccountId { get; set; }
        public int GroupId { get; set; }
        public int AccountGroupId { get; set; }
        public int TagGroupId { get; set; }
        public string RawId { get; set; }
        public string Name { get; set; }
        public string TagId { get; set; }
    }
}
