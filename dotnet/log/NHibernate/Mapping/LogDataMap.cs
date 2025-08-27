using FluentNHibernate.Mapping; 

namespace log.NHibernate.Mapping
{
    class LogDataMap : ClassMap<LogData>
    {
        public LogDataMap()
        {
            Id(p => p.Id).GeneratedBy.Identity();
            Map(p => p.StartTime).Index("");
            Map(p => p.Date).Not.Nullable().Index("");
            Map(p => p.Message).CustomSqlType("text");
            Map(p => p.Module).Length(100);
            Map(p => p.Action).Length(500);
            Map(p => p.QueryParams).Length(500);
            Map(p => p.HttpMethod).Length(10);
            Map(p => p.HttpResponseCode);
            Map(p => p.GroupId).Length(100);
            Map(p => p.ClientAddress).Length(50);
            Map(p => p.HostName).Length(50);
            References(p => p.Category).Not.Nullable().Cascade.None();
            References(p => p.SubCategory).Not.Nullable().Cascade.None();
            Map(p => p.UserId).Index("");
        }
    }

    class CategoryMap : ClassMap<Category>
    {
        public CategoryMap()
        {
            Id(p => p.Id).GeneratedBy.Assigned();
            Map(p => p.Description).Not.Nullable().Unique().Length(100);
        }
    }

    class SubCategoryMap : ClassMap<SubCategory>
    {
        public SubCategoryMap()
        {
            Id(p => p.Id).GeneratedBy.Assigned();
            Map(p => p.Description).Not.Nullable().Unique().Length(100);
            References(p => p.Category).Cascade.All();
        }
    }
}
