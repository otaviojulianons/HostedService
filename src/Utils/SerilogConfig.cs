﻿namespace HostedService.Utils
{
    public class SerilogConfig
    {
        public SerilogElasticConfig Elasticsearch { get; set; }
        public SerilogFileConfig File { get; set; }
    }
}
