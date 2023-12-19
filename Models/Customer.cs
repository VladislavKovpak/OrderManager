using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TaskAuthenticationAuthorization.Models
{
    public enum Discount
    {
        O, R, V
    }
    public class Customer
    {
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public Discount? Discount { get; set; }
        public ICollection<Order> Orders { get; set; }

        [ValidateNever]
        public string? Email { get; set; }

        [ValidateNever]
        public string? Password { get; set; }

        [ForeignKey("Role")]
        public int? RoleId { get; set; }
        public Role? Role { get; set; }
    }
}
