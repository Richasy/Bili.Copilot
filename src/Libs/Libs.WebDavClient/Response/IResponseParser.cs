﻿//<auto-generated/>
namespace WebDav
{
    internal interface IResponseParser<out TResponse>
    {
        TResponse Parse(string response, int statusCode, string description);
    }
}
