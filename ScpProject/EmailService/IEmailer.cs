using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
   public interface IEmailer
    {
        void Send(string contents, string to, string from);
        
    }
}
