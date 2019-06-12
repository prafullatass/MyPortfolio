document.getElementById("UserAgencyId").addEventListener("change", () => {
    var agencyId = document.getElementById("UserAgencyId");
    //var connectionString = "Server=localhost\\SQLExpress;Database=MyPortfolio;Trusted_Connection=True;";
    //var connection = new ActiveXObject("ADODB.Connection");

    //connection.Open(connectionString);
    //var rs = new ActiveXObject("ADODB.Recordset");

    //    rs.Open(`SELECT * FROM Stock where CountryId = ${agencyId}`, connection);
    //    rs.MoveFirst
    //    while (!rs.eof) {
    //        document.write(rs.fields(1));
    //        rs.movenext;
    //    }

    //    rs.close;
    //    connection.close; 

    //var section = S("")
    // var opt = $("<option></option>");
    //$.ajax({
    //    url: `Stocks/GetStocksOfCountry/${agencyId}`,
    //    datatype: "JSON",
    //    type: "Get",
    //    success: function (data) {
    //        debugger;
    //        for (i = 0; i < data.length; i++) {
    //            var opt = new Option(data[j].name)

    //        }
    //    }
    var id = parseInt(agencyId.value);
    fetch(`\GetStocksFromUserAgencyId?_id=${id}`)
        .then (r => r.json())
        .then(response => {
            $("#StockId").empty();
            for (i = 0; i < response.length; i++) {
                $("#StockId").append(`<option value = "${response[i].stockId}">${response[i].name}</option>`);
            }
    });
})