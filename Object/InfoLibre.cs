using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebservicesSage.Object
{
    [Serializable()]
    public class InfoLibre
    {
        public string Libelle { get; set; }
        public string Value { get; set; }

        public InfoLibre(string libelle, string value)
        {
            Libelle = libelle;
            Value = value;
        }

        public InfoLibre()
        {

        }
    }
}
