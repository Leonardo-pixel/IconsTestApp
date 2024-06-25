using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconsTestApp.Models
{
    public class TestItem
    {
        public TestItem(string name, string description, bool firstTwoIconsReversed, bool hasThirdIcon)
        {
            Name = name;
            Description = description;
            HasThirdIcon = hasThirdIcon;
            FirstTwoIconsReversed = firstTwoIconsReversed;
            if (firstTwoIconsReversed)
            {
                ImageSource1 = "test_icon2.png";
                ImageSource2 = "test_icon1.png";
            }
            else
            {
                ImageSource1 = "test_icon1.png";
                ImageSource2 = "test_icon2.png";
            }
            if (hasThirdIcon)
            {
                ImageSource3 = "test_icon3.png";
            }
        }

        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool HasThirdIcon { get; private set; }
        public bool FirstTwoIconsReversed { get; private set; }

        public string? ImageSource1 { get; set; }
        public string? ImageSource2 { get; set; }
        public string? ImageSource3 { get; set; }
    }
}
