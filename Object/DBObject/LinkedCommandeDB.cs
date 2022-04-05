using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Objets100cLib;

namespace WebservicesSage.Object.DBObject
{
    public class LinkedCommandeDB
    {
        public int Id { get; set; }
        public string OrderID { get; set; }
        public DocumentType OrderType { get; set; }
        public string CT_Num { get; set; }
        public string DO_piece { get; set; }
        public string DO_Ref { get; set; }
    }
}
