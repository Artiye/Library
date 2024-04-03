using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Domain.Entity
{
    public class EncryptionSettings
    {
        public string AesKey { get; set; }
        public string HmacKey { get; set; }
        public string Iv { get; set; }
    }
}
