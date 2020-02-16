using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProcastinationKiller.Models
{
    public class RegistartionCode
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime? ConfirmationDate { get; set; }

        public void Confirm()
        {
            if (IsConfirmed)
                throw new InvalidOperationException("This code is already confirmed.");

            ConfirmationDate = DateTime.Now;
            IsConfirmed = true;
        }
    }
}
