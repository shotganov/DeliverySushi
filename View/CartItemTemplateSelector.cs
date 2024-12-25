using DeliverySushi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DeliverySushi.View
{
    public class CartItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SushiTemplate { get; set; }
        public DataTemplate SetTemplate { get; set; }
        public DataTemplate AddonTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Sushi)
            {
                return SushiTemplate;
            }
            else if (item is Set)
            {
                return SetTemplate;
            }
            else if (item is Addon)
            {
                return AddonTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
