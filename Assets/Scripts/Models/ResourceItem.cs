using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [System.Serializable]
    public class ResourceItem<T>
    {
        public T Value;                     //The resource itself (e.g., Sprite, AudioClip)
        public List<ResourceParameter> Parameters; //Parameters loaded from the JSON file
    }
}
