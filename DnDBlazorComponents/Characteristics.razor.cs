using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using NeoBlazorphic.StyleParameters;

namespace DnDBlazorComponents
{
    public partial class Characteristics : ComponentBase
    {


        // UI Methods

        private static BorderRadius _round { get; set; } = new BorderRadius(5, "em");
        public BorderRadius GetBonusMarkerBorderRadius()
        {
            return _round;
        }
    }
}
