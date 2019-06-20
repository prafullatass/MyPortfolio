document.getElementById("UserAgencyId").addEventListener("change", () => {
    var agencyId = document.getElementById("UserAgencyId");
    var id = parseInt(agencyId.value);
    fetch(`\GetStocksFromUserAgencyId?_id=${id}`)
        .then (r => r.json())
        .then(response => {
            $("#StockId").empty();
            $("#StockId").append("<option value=null>Choose stock --- </option>");
            for (i = 0; i < response.length; i++) {
                $("#StockId").append(`<option value = "${response[i].stockId}">${response[i].name}</option>`);
            }
    });
})

