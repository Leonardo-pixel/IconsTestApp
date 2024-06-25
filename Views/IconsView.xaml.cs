using IconsTestApp.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;

namespace IconsTestApp.Views
{
    public partial class IconsView : ContentPage
    {
        private List<TestItem>? _items;
        public List<TestItem>? Items 
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        public Command ItemTappedCommand { get; set; }

        public IconsView()
        {
            InitializeComponent();
            ItemTappedCommand = new Command(OnItemTapped);
            BindingContext = this;
            CreateItems();
        }

        private void OnItemTapped(object obj)
        {
            if (obj is not TestItem item)
            {
                return;
            }

            bool hasThirdIcon = item.HasThirdIcon;
            //bool firstTwoIconsReversed = item.FirstTwoIconsReversed;
            string shouldnt = hasThirdIcon ? "" : "n't";
            DisplayAlert("Item tapped", $"Tapped item {item.Name} should{shouldnt} have a third icon!", "OK");
        }

        // Random name generation, doesn't really matter.
        private readonly string[] l = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"];
        private string GetName(int i)
        {
            return $"{l[i % 26]}{l[(i*2) % 26]}{l[(i*3) % 26]}{l[(i*4) % 26]}{l[(i*5) % 26]}{l[(i*6) % 26]}{l[(i*7) % 26]}{l[(i*8) % 26]}";
        }

        private void CreateItems()
        {
            if (Items == null)
            {
                Items = new List<TestItem>();
            }
            
            // Create TestItems with some deviation in the icons they will display.
            bool hasThirdIcon = false;
            bool firstTwoIconsReversed = true;
            // After i iterations, switch wether or not a third icon is added to the new TestItems.
            int[] iValues = [3, 5, 10, 6, 12, 3, 18, 9, 22, 2, 10];
            // Number of iterations after which the first and second icon are reversed, for some extra deviation. Not really important.
            int[] reversingValues = [3, 5, 9, 15, 17, 18, 26, 29, 30, 35, 50, 55, 61, 67, 72, 79, 83, 85, 90, 96];
            int reversingIndex = 0;
            int total = 0;
            foreach (int i in iValues)
            {
                int j = 0;
                while (j < i)
                {
                    Items.Add(new TestItem(name: GetName(total), description: "Exactly the same description!", firstTwoIconsReversed, hasThirdIcon));
                    if (total == reversingValues[reversingIndex])
                    {
                        firstTwoIconsReversed = !firstTwoIconsReversed;
                        if (reversingIndex < (reversingValues.Length - 1))
                        {
                            reversingIndex++;
                        }
                    }
                    total++;
                    j++;
                }
                hasThirdIcon = !hasThirdIcon;
            }

            OnPropertyChanged(nameof(Items));
        }
    }

}
