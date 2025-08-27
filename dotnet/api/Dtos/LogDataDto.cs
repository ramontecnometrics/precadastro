using System;
using log;

namespace api.Dtos
{
    public class LogDataDto
    {
        public long Id { get; set; }
        public SubCategoryDto SubCategory { get; set; }
        public DateTime? Date { get; set; }
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

        public static LogDataDto Build(LogData item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new LogDataDto()
            {
                Id = item.Id,
                Action = item.Action,
                ClientAddress = item.ClientAddress,
                Date = item.Date,
                GroupId = item.GroupId,
                HostName = item.HostName,
                HttpMethod = item.HttpMethod,
                HttpResponseCode = item.HttpResponseCode,
                Message = item.Message,
                Module = item.Module,
                QueryParams = item.QueryParams,
                SubCategory = SubCategoryDto.Build(item.SubCategory),
                UserId = item.UserId
            };

            return result;
        }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public static CategoryDto Build(Category item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new CategoryDto()
            {
                Id = item.Id,
                Description = item.Description
            };
            return result;
        }
    }

    public class SubCategoryDto
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public static SubCategoryDto Build(SubCategory item)
        {
            if (item == null)
            {
                return null;
            }

            var result = new SubCategoryDto()
            {
                Id = item.Id,
                Description = item.Description
            };
            return result;
        }
    }
}
