using System;
using System.Collections.Generic; 

namespace log
{
    public class LogData
    {
        public Category Category { get; set; }
        public SubCategory SubCategory { get; set; }
        public long Id { get; internal set; }
        public DateTime StartTime { get; set; }
        public DateTime Date { get; set; }
        public string Module { get; set; }
        public string Action { get; set; }
        public string QueryParams { get; set; }
        public string Message { get; set; }
        public string HostName { get; set; }
        public string HttpMethod { get; set; }
        public int? HttpResponseCode { get; set; }
        public string GroupId { get; set; }
        public string ClientAddress { get; set; }
        public int? UserId { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }

    public class SubCategory
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
    }

    public class LogSetup
    {
        public bool Enabled { get; set; }
        public string ConnectionString { get; set; }
        public IEnumerable<Category> AvailableCategories { get; set; }
        public IEnumerable<SubCategory> AvailableSubCategories { get; set; }
        public string HostName { get; set; }
    }
}
