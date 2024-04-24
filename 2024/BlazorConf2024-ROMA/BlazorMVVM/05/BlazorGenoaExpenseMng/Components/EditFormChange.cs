using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;

namespace BlazorGenoaExpenseMng.Components
{
    public partial class EditFormChange : EditForm
    {
        [Parameter] public EventCallback OnPropertyChange { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            EditContext.OnFieldChanged -= EditContext_OnFieldChanged; // Unsuscribe old parameters set
            EditContext.OnFieldChanged += EditContext_OnFieldChanged;
            
        }
        
        private void EditContext_OnFieldChanged(object sender, FieldChangedEventArgs e)
        {
            if (EditContext.Validate())
                OnPropertyChange.InvokeAsync();
        }
    }
}
