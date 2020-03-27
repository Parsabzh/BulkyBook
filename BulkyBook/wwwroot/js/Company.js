var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "10%" },
            { "data": "streetAddress", "width": "10%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return '<input type="checkbox" disabled checked />';
                    }
                    else {
                        return '<input type="checkbox" disabled  />';
                    }
                },
                "width": "10%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return ` <div class="text-center">
    <a href="/Admin/Company/CompanyUpsert/${data}" class="btn btn-success text-white" style="cursor: pointer">
        <i class="fas fa-edit"></i>
    </a>
    <a onclick=Delete("/Admin/Company/Delete/${data}") class="btn btn-danger text-white" style="cursor: pointer">
        <i class="fas fa-trash-alt"></i>
    </a>
</div>`;


                }, "width": "25%"
            }
        ]

    });
}

var swalWithBootstrapButtons = Swal.mixin({
    customClass: {
        confirmButton: 'btn btn-success',
        cancelButton: 'btn btn-danger'
    },
    buttonsStyling: false
});
function Delete(url) {
    swalWithBootstrapButtons.fire({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore the data!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, cancel!',
        reverseButtons: true,
        dangerMode: true
    }).then((result) => {
        if (result.value) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else if (result.dismiss === Swal.DismissReason.cancel) {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}