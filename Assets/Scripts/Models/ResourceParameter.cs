using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ResourceParameter
    {
        public string Key;   //The parameter name (e.g., "Role", "Description")
        public string Value; //The parameter entry (e.g., "Tank", "A brave warrior")
    }

    [System.Serializable]
    public class ResourceParameterList
    {
        public List<ResourceParameter> Parameters = new List<ResourceParameter>();
    }
}
