var dataTable;

//let table = new DataTable('#myTable');

$(document).ready(function () {
    loadDataTable()
})


function loadDataTable() {
    dataTable = $("#tblData").dataTable({
        "ajax": {
            url: "/inquiry/GetInquiryList"
        },
        columns: [
            { "data": "id", "width": "10" },
            { "data": "fullname", "width": "15" },
            { "data": "phoneNumber", "width": "15" },
            { "data": "email", "width": "15" },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/inquiry/Details/${data}" class="btn btn-success text-white" style="cursor:pointer">
                                <i class="fas fa-edit"></i>
                            </a>
                        </div>
                    `;
                },
                "width" : "5%"
            }
        ]
    });
}