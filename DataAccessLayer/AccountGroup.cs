namespace VypinApi.DataAccessLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AccountGroup")]
    public partial class AccountGroup
    {
        public int AccountGroupId { get; set; }
        public int AccountId { get; set; }
        public int GroupId { get; set; }
    }
}
