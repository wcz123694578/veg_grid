using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using VegGridLayouter.UI.Events;

namespace VegGridLayouter.UI.Behaviors
{
    public class TextBoxBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.TextChanged += OnTextChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Lock_;
                StaticVariable.eventAggregator.GetEvent<UpdateXmlEvent>().Publish(new UpdateXmlEventModel());
                StaticVariable.TreeViewState = StaticVariable.TreeViewStateType.Unlock_;
            }
        }
    }
}
