
document.getElementById("Agency_CountryId").addEventListener("change", () => {
    var agencyId = document.getElementById("Agency_CountryId");
    var id = parseInt(agencyId.value);
    fetch(`\GetCountrysAgencies?_id=${id}`)
        .then(r => r.json())
        .then(response => {
            $("#Agencies").empty();
            $("#Agencies").append("<option value=null>Choose stock</option>");
            for (i = 0; i < response.length; i++) {
                $("#Agencies").append(`<option value = "${response[i].agencyId}">${response[i].name}</option>`);
            }
        });
})
