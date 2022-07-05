var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Weeks/GetAll"
        },
        "columns": [
            { "data": "weekNumber", "width": "10%" },
            { "data": "expectedWeight", "width": "10%" },
            { "data": "averageWeight", "width": "10%" },
            { "data": "weeklyLoss", "width": "10%" },
            { "data": "weeklyCalories", "width": "10%" },

        ],
        responsive: true
    });
}