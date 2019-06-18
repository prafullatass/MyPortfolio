document.getElementById("transactions").addEventListener("click", () => {
    if (document.getElementById("transactions").textContent == "Show All Transactions") {
        document.getElementById("transactionsDetails").style.display = 'inline';
        document.getElementById("transactions").textContent = "Hide Transactions";
    }
    else {
        document.getElementById("transactionsDetails").style.display = 'none';
        document.getElementById("transactions").textContent = "Show All Transactions";
    }
});