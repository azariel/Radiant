using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RadiantReader.Views
{
    /// <summary>
    /// Interaction logic for ReaderContentUserControl.xaml
    /// </summary>
    public partial class ReaderContentUserControl : UserControl
    {
        public ReaderContentUserControl()
        {
            InitializeComponent();
        }

        public void SetTextContent(List<Inline> aLineElements)
        {
            // Add line elements to textblock
            TextContentTextBlock.Inlines.Clear();
            TextContentTextBlock.Text = "";// Start by emptying it just to be sure we don't forget anything
            TextContentTextBlock.Inlines.AddRange(aLineElements);
        }
    }
}
