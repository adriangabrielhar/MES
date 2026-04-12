using System;
using System.Collections.Generic;
using System.Text;

namespace MainApplication.DTO
{
    public class ResponseData
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int CreateOrderId { get; set; }
    }
}
