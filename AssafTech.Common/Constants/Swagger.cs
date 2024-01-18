namespace AssafTech.Common.Constants;

public struct Swagger
{
    public struct DocumentTitle
    {
        public const string DataList = "Datalist documentation";
        public const string CRM = "CRM documentation";
        public const string Gateway = "Gateway documentation";
    }

    public struct Title
    {
        public const string DataList = "DataList API";
        public const string CRM = "CRM API";
        public const string Gateway = "Gateway API";
    }

    public struct Version
    {
        public const string V1 = "v1";
        public const string V2 = "v2";
        public const string V3 = "v3";
    }

    public struct HeaderKey
    {
        public const string Authorization = "Authorization";
    }

    public struct Description
    {
        public const string SecurityScheme = "Please enter a valid token";
    }
}
