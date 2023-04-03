using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frequencio
{
    public class AudioAttributesList
    {
        public AudioAttributesList()
        {
            AudioAttributes = new List<AudioAttributes>();
        }

        public void Add(AudioAttributes audioAttributes)
        {
            AudioAttributes.Add(audioAttributes);
        }

        public List<AudioAttributes> AudioAttributes { get; set; }
    }
}
