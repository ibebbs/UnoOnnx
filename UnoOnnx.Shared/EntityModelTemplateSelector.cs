using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace UnoOnnx
{
    public class EntityModelTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item switch
            {
                EntityModel em when em.Label.Equals("B-PER") => Person,
                EntityModel em when em.Label.Equals("I-PER") => Person,
                EntityModel em when em.Label.Equals("B-ORG") => Organisation,
                EntityModel em when em.Label.Equals("I-ORG") => Organisation,
                EntityModel em when em.Label.Equals("B-LOC") => Location,
                EntityModel em when em.Label.Equals("I-LOC") => Location,
                EntityModel em when em.Label.Equals("B-MISC") => Misc,
                EntityModel em when em.Label.Equals("I-MISC") => Misc,
                EntityModel em => None,
                _ => base.SelectTemplateCore(item)
            };
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return SelectTemplateCore(item);
        }
        public DataTemplate Person { get; set; }

        public DataTemplate Organisation { get; set; }

        public DataTemplate Location { get; set; }

        public DataTemplate Misc { get; set; }

        public DataTemplate None { get; set; }
    }
}
