
function loadPartialSolicitudes() {
   window.location.href = '/Solicitudes/Index';
};

function getAccionView(id) {
    window.location.href = '/Solicitudes/Accion?solicitudId=' + id;
};

function getAddOrEditView(id) {
    window.location.href = '/Solicitudes/AddOrEdit/' + id;
};

//Get All User
function GetGataSolicitudes() {
    $('#SolicitudesT').DataTable({
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
            url: '/Solicitudes/GetAllData',
            type: 'POST',
            contentType: "application/json",
            dataType: "json",
            data: function (data) {
                return JSON.stringify(data);
            }
        },
        columns: [
            {
                data: "parcela",
                render: function (data, type, row) {
                    return `<a href="#" onClick="getAddOrEditView(${row.id})" class="text-dark" style="cursor:pointer">${data}</a>`;
                }
            },
            {
                data: "cantidad",
                render: function (data, type, row) {
                    return `<a href="#" onClick="getAddOrEditView(${row.id})" class="text-dark" style="cursor:pointer">${data}</a>`
                }
            },
            {
                data: "beneficiario",
                render: function (data, type, row) {
                    return `<a href="#" onClick="getAddOrEditView(${row.id})" class="text-dark" style="cursor:pointer">${data}</a>`;
                }

            },
            { data: "fechaEntrada" },
            {
                data: "manzana",
                render: function (data, type, row) {
                    return `<a href="#" onClick="getAddOrEditView(${row.id})" class="text-dark" style="cursor:pointer">${data}</a>`;
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
                data: "status",
                render: function (data, type, row) {
                    var val = 0;
                    if (data == 0) {
                        val = 25;
                    } else if (data == 1) {
                        val = 50;
                    } else if (data == 2) {
                        val = 75;
                    
                    } else if (data == 3) {
                        val = 100;
                    }
                    return `
                    <div class="progress bg-secondary">
                      <div class="progress-bar progress-bar-striped progress-bar-animated pl-0 pr-0"
                           role="progressbar" aria-valuenow="${val}" aria-valuemin="0" aria-valuemax="100"
                           style="width: ${val}%">${val}% 
                      </div>
                    </div>`;
                }
            },
        ],
    });
}

function GetSelects(id) {    
    $.ajax({
        type: 'GET',
        url: '/Solicitudes/GetSolicitudById/' + id,
        success: function (res) {
            console.log(res);
            //Llena el select Municipio
            var $newOption = $("<option selected='selected'></option>")
                .val(res.municipioId)
                .text(res.provincia + " - " + res.municipio)
            $("#MunicipioId").append($newOption).trigger('change');
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

//AddOrEditGet
showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
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


function GetDetails(id) {
    $.ajax({
        type: 'GET',
        url: "/Solicitudes/Details?id=" + id,
        success: function (res) {
            $('#details-modal .modal-body').html(res);
            $('#details-modal').modal('show');
        }
    })
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

function Acciones(id) {
    var tr = '';
    $.ajax({
        url: '/Solicitudes/Acciones/' + id,
        method: 'GET',
        success: (result) => {
            $.each(result, (k, v) => {
                var d = v.create.split('T')[0]; 
                var date = d.split('-');
                date = date[2] + "/" + date[1] + "/" + date[0];
                tr = tr + `<tr>
                        <td class="align-middle" style="max-width: 300px; overflow-wrap: break-word; word-wrap: break-word; white-space: normal !important;">${v.nombre}</td>
                        <td class="align-middle" style="max-width: 300px; overflow-wrap: break-word; word-wrap: break-word; white-space: normal !important;">${v.descripcion}</td>
                        <td class="align-middle">${v.name}</td>
                        <td class="align-middle">${date}</td>
                    </tr>`;
            });

            $("#tableBodyAccion").html(tr);
            //Notification

        },
        error: (error) => {
            console.log(error);
        }
    });
}

