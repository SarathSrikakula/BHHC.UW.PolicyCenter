using Dapper;
using System.Data.SqlClient;
using System.Xml.Linq;

public async Task<FormControl> ParseControlAsync(string xml, SqlConnection connection)
{
    var xdoc = XDocument.Parse(xml);

    var controlType = xdoc.Root.Name.LocalName;

    var formControl = new FormControl
    {
        Id = Guid.NewGuid().ToString(), // or from DB
        Label = xdoc.Root.Attribute("Label")?.Value ?? controlType,
        ControlType = controlType,
        Attributes = new Dictionary<string, object>()
    };

    // Step 1: Load all XML elements into Attributes
    foreach (var element in xdoc.Root.Elements())
    {
        formControl.Attributes[element.Name.LocalName] = element.Value;
    }

    // Step 2: Special handling for <Select><Type>Dropdown</Type>
    if (formControl.ControlType.Equals("Select", StringComparison.OrdinalIgnoreCase)
        && formControl.Attributes.TryGetValue("Type", out var typeVal)
        && typeVal?.ToString().Equals("Dropdown", StringComparison.OrdinalIgnoreCase) == true
        && formControl.Attributes.ContainsKey("Query"))
    {
        var query = formControl.Attributes["Query"].ToString();

        var rows = (await connection.QueryAsync(query)).ToList();

        var options = rows.Select(row =>
        {
            var dict = new Dictionary<string, object>();
            foreach (var kv in (IDictionary<string, object>)row)
                dict[kv.Key] = kv.Value;
            return dict;
        }).ToList();

        // Replace Query with actual options
        formControl.Attributes.Remove("Query");
        formControl.Attributes["options"] = options;
    }

    return formControl;
}
