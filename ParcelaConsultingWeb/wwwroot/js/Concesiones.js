function loadPartialSolicitudes() {
    window.location.href = '/Concesiones/Index';
};


//Get All User
function GetConceciones() {
    $('#ConcesionesT').DataTable({
        "destroy": true,
        "autoWidth": true,
        "processing": true,
        "serverSide": true,
        "paging": true,
        "searching": { regex: true },
        "language": {
            "url": "/plugins/datatables/lang/Spanish.json"
        },
        "dom": 'frtip',
        ajax: {
            url: '/Concesiones/GetAllData',
            type: 'POST',
            contentType: "application/json",
            dataType: "json",
            data: function (data) {
                return JSON.stringify(data);
            }
        },
        columns: [
            {
                data: "noExpediente",
                
            },
            {
                data: "remitente",
                
            },
            {
                data: "asunto",
                
            },
            {
                data: "asignado",

            },

            {
                data: "fechaAsignacion",
                render: function (data, type, row) {
                    var fecha = "";
                    if (data == "1/1/0001") {
                        fecha = "N/A";
                    } else {
                        fecha = data;
                    }
                    return `<text class="text-center">${fecha}</text>`
                }

            },

            {
                data: "fechaEnviado",
                render: function (data, type, row) {
                    var fecha = "";
                    if (data == "1/1/0001") {
                        fecha = "N/A";
                    } else {
                        fecha = data;
                    }
                    return `<text class="text-center">${fecha}</text>`
                }

            },

            {
                data: "diarenaNo",

            },

            {
                data: "departamento",
                render: function (data, type, row) {
                    return `<a href="#" onClick="getAddOrEditView(${row.id})" class="text-dark" style="cursor:pointer">${data}</a>`;

                }
           
            },

            {
                data: "id",
                render: function (data, type, row) {
                    return `<a class="btn-group" onclick="GetDetails(${data})" class="text-dark pl-1" style="cursor:pointer">
                                <i class="fas fa-user-plus"></i>
                            </a>`;
                }
           
            },
          
        ],
    });
}


function GetDetails(id) {
    
    $.ajax({
        type: 'GET',
        url: "/Concesiones/Details?id=" + id,
        success: function (res) {
            console.log(res);
            $('#details2-modal').modal('show');
        }
    })
}


function GetSelects(id) {
    $.ajax({
        type: 'GET',
        url: '/Concesiones/GetSolicitudById/' + id,
        success: function (res) {
            console.log(res);
            //Llena el select Departamento
            var $newOption = $("<option selected='selected'></option>")
                .val(res.departamentoId)
                .text(res.departamrnto)
            $("#DepartamentoId").append($newOption).trigger('change');
        }
    })

}


//Metodo POST para eliminar
function deleteRow(id, router) {
    Swal.fire({
        title: 'Estas Seguro?',
        text: "Esta Acción no se podra revertir!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'Cancelar',
        confirmButtonText: 'Si, Eliminar!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: router,
                data: { 'id': id },
                success: function (data) {
                    if (data.isValid) {
                        Swal.fire(
                            'Eliminado!',
                            data.message,
                            'success'
                        )
                        GetGataSolicitudes();
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'No se puede Eliminar',
                            text: data.message,
                        })
                    }
                }
            })
        }
    })
}


//AddOrEditPost
jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                debugger;
                if (res.isValid) {
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    //loadPartialSolicitudes();
                    Swal.fire(
                        'Bien!',
                        res.message,
                        'success'
                    )
                    GetGataSolicitudes();
                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function salida(id, router) {
    Swal.fire({
        title: 'Estas Seguro?',
        text: "Se le dara salida a esta solicitud, Esta Acción no se podra revertir!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'Cancelar',
        confirmButtonText: 'Dar Salida!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: router,
                data: { 'id': id },
                success: function (data) {
                    if (data.isValid) {
                        Swal.fire(
                            'Salida Existosa!',
                            data.message,
                            'success'
                        )
                        GetGataSolicitudes();
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'No se puede dar salida!',
                            text: data.message,
                        })
                    }
                }
            })
        }
    })
}

