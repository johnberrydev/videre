﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Videre.Core.ContentProviders
{
    public interface IWidgetContentProvider
    {
        string GetJson(List<string> ids);
        T Get<T>(List<string> ids) where T : class;
        List<string> Save(string json);
        Dictionary<string, string> Import(string portalId, string json, Dictionary<string, string> idMap); //return <oldId, newId>
        void Delete(List<string> ids);
    }
}
