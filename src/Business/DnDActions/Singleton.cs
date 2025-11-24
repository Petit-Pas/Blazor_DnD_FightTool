using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor;

namespace DnDFightTool.Business.DnDActions
{
    public static class Singleton
    {
        public static IDialogService SingletonDialogService { get; set; }
    }
}
