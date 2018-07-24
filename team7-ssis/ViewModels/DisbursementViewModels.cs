﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace team7_ssis.ViewModels
{
    public class DisbursementFormViewModel
    {
        public string DisbursementId { get; set; }
        public string Representative { get; set; }
        public string Department { get; set; }
        public string OrderTime { get; set; }
        public string CollectionPoint { get; set; }
    }

    public class DisbursementFormTableViewModel
    {
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
    }
}