using System;
using data;

namespace model.Repositories
{
    public class ClearIdleSessionsRepository
    {
        public ISqlCommand Command { get; }

        public ClearIdleSessionsRepository(ISqlCommand command)
        {
            Command = command;
        }

        public virtual void Execute()
        {
            Command.ExecuteNonQuery(new CommandData()
            {
                Sql = @"
WITH inactive_connections AS (
    SELECT
        pid,
        rank() over (partition by client_addr order by backend_start ASC) as rank
    FROM 
        pg_stat_activity
    WHERE
        -- Exclude the thread owned connection (ie no auto-kill)
        pid <> pg_backend_pid( )
    AND
        -- Exclude known applications connections
        application_name !~ '(?:psql)|(?:pgAdmin.+)'
    AND
        -- Include connections to the same database the thread is connected to
        datname = current_database() 
    AND
        -- Include connections using the same thread username connection
        usename = 'usertecnometrics' 
    AND
        -- Include inactive connections only
        state in ('idle') 
)
SELECT
    pg_terminate_backend(pid)
FROM
    inactive_connections 
WHERE
    rank > 1 -- Leave one connection for each application connected to the database                
                "
            });
        }

    }
}
