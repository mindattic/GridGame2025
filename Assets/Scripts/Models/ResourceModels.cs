using System;
using System.Collections.Generic;

namespace Assets.Scripts.Models
{



    [Serializable]
    public class ResourceItem<T>
    {
        public T Value;                     
        public List<ResourceParameter> Parameters = new List<ResourceParameter>();
    }

    [Serializable]
    public class ResourceParameter
    {
        public string Key;  
        public string Value;
    }

    [Serializable]
    public class ResourceParameterList
    {
        public List<ResourceParameter> Parameters = new List<ResourceParameter>();
    }

}
