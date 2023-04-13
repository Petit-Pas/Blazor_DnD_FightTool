using Microsoft.AspNetCore.Components;

namespace SharedComponents.Modals.Common
{
    public partial class ModalBase
    {
        [Parameter]
        public RenderFragment? ModalActions { get; set; }
        [Parameter]
        public RenderFragment? ModalHeader { get; set; }
        [Parameter]
        public RenderFragment? ModalBody { get; set; }
        [Parameter]
        public RenderFragment? ModalFooter { get; set; }
    }
}
